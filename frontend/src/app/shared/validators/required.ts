import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function required(errorName: string, errorMessage: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    return !value ? { [errorName]: errorMessage } : null;
  };
}
