import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function required(errorMessage: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    return !value ? { ['required']: errorMessage } : null;
  };
}
