import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-input',
  standalone: true,
  templateUrl: './input.component.html',
})
export class InputComponent {
  @Input() label?: string;
  @Input() name?: string;
  @Input() autocomplete?: string;
  @Input() required?: boolean;
  @Input() disabled: boolean = false;
  @Input() readonly?: boolean;
  @Input() placeholder?: string | undefined;
  @Input() inputType?: 'text' | 'number' | 'password';
  @Input() inputClass?: string = '';
}
