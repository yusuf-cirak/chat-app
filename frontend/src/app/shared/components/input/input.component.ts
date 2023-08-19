import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [ReactiveFormsModule],
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
  @Input() parentGroup!: FormGroup;

  @Input() controlName!: string;
}
