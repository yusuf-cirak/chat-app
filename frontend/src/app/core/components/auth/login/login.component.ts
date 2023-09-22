import { RegisterUserDto } from './../../../dtos/register-user-dto';
import { NgIf } from '@angular/common';
import { Component, WritableSignal, inject, signal } from '@angular/core';
import {
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppState } from 'src/app/app.state';
import { ButtonComponent } from 'src/app/shared/components/button/button.component';
import { InputComponent } from 'src/app/shared/components/input/input.component';
import { loginAction } from 'src/app/shared/states/user/user.actions';

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
    userName: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  // Global state
  private readonly store = inject(Store<AppState>);

  submitForm(formValues: RegisterUserDto) {
    this.isFormSubmitted.set(true);

    this.store.dispatch(loginAction({ user: formValues as any }));
  }
}
