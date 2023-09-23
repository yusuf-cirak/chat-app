import { RegisterUserDto } from './../../../dtos/register-user-dto';
import { NgIf } from '@angular/common';
import { Component, WritableSignal, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppState } from 'src/app/app.state';
import { ButtonComponent } from 'src/app/shared/components/button/button.component';
import { InputComponent } from 'src/app/shared/components/input/input.component';
import { loginAction } from 'src/app/shared/states/user/user.actions';
import { minLength } from 'src/app/shared/validators/min.length';
import { required } from 'src/app/shared/validators/required';

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

  // Global state
  private readonly store = inject(Store<AppState>);

  login(formValid: boolean, formValues: RegisterUserDto) {
    if (!formValid) {
      // validateAllFormFields(this.formGroup);
      this.formGroup.markAllAsTouched();
      return;
    }

    this.isFormSubmitted.set(true);

    this.store.dispatch(loginAction({ user: formValues as any }));
  }
}
