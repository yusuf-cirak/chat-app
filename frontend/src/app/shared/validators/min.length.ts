import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function minLength(
  length: number,
  errorName: string,
  errorMessage: string
): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    const valid = value && value.length >= length;

    return !valid ? { [errorName]: errorMessage } : null;
  };
}
