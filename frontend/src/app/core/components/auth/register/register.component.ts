import { Component, WritableSignal, inject, signal } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { LoginUserDto } from 'src/app/core/dtos/login-user-dto';
import { ButtonComponent } from 'src/app/shared/components/button/button.component';
import { InputComponent } from 'src/app/shared/components/input/input.component';
import { minLength } from 'src/app/shared/validators/min.length';
import { required } from 'src/app/shared/validators/required';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, ButtonComponent, InputComponent],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
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

  register(formValid: boolean, formValues: LoginUserDto) {
    if (!formValid) {
      this.formGroup.markAllAsTouched();
      return;
    }

    this.isFormSubmitted.set(true);
  }
}
