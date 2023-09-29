import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgIf } from '@angular/common';
import {
  Component,
  DestroyRef,
  WritableSignal,
  inject,
  signal,
} from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LoginUserDto } from 'src/app/core/dtos/login-user-dto';
import { AuthService } from 'src/app/core/services/auth.service';
import { ButtonComponent } from 'src/app/shared/components/button/button.component';
import { InputComponent } from 'src/app/shared/components/input/input.component';
import { minLength } from 'src/app/shared/validators/min.length';
import { TokenService } from 'src/app/core/services/token.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    RouterLink,
    NgIf,
    InputComponent,
    ButtonComponent,
    ReactiveFormsModule,
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  // Inject dependencies
  private readonly authService = inject(AuthService);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  private readonly toastrService = inject(ToastrService);
  private readonly _destroyRef = inject(DestroyRef);

  // Form states
  isFormSubmitted: WritableSignal<boolean> = signal(false);

  private readonly formBuilder = inject(NonNullableFormBuilder);
  formGroup = this.formBuilder.group({
    userName: [
      '',
      [minLength(3, 'Username must be at least 3 characters long')],
    ],
    password: [
      '',
      [minLength(6, 'Password must be at least 6 characters long')],
    ],
  });

  async login(formValid: boolean, user: LoginUserDto) {
    if (!formValid) {
      // validateAllFormFields(this.formGroup);
      this.formGroup.markAllAsTouched();
      return;
    }
    this.isFormSubmitted.set(true);

    this.authService
      .login(user)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (tokens) => {
          this.tokenService.setTokens(tokens);
          const decodedToken = this.tokenService.decodeAccessToken(
            tokens.accessToken
          );
          const currentUser =
            this.tokenService.getUserCredentialsFromDecodedToken(decodedToken);
          this.authService.setUser(currentUser!);
          this.router.navigateByUrl('/chat');
        },
        error: (error) => {
          this.toastrService.error(
            error?.error?.detail || 'Something went wrong'
          );
          this.isFormSubmitted.set(false);
          this.formGroup.reset();
        },
      });
  }
}
