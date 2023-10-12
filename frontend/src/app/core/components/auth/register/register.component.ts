import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  Component,
  DestroyRef,
  WritableSignal,
  inject,
  signal,
} from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { RegisterUserDto } from 'src/app/core/dtos/register-user-dto';
import { AuthService } from 'src/app/core/services/auth.service';
import { ButtonComponent } from 'src/app/shared/components/button/button.component';
import { InputComponent } from 'src/app/shared/components/input/input.component';
import { minLength } from 'src/app/shared/validators/min.length';
import { TokenService } from 'src/app/core/services/token.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, ButtonComponent, InputComponent],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  // Inject dependencies
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toastrService = inject(ToastrService);
  private readonly tokenService = inject(TokenService);
  private readonly _destroyRef = inject(DestroyRef);
  isFormSubmitted: WritableSignal<boolean> = signal(false);

  formBuilder = inject(NonNullableFormBuilder);

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

  register(formValid: boolean, user: RegisterUserDto) {
    if (!formValid) {
      this.formGroup.markAllAsTouched();
      return;
    }
    this.isFormSubmitted.set(true);

    this.authService
      .register(user)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (tokens) => {
          this.tokenService.setTokensAndDecodeAccessToken(tokens);
          const currentUser =
            this.tokenService.getUserCredentialsFromDecodedToken();
          this.authService.setUser({
            ...currentUser!,
            lastUpdateDate: new Date(Date.now()),
          });
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
