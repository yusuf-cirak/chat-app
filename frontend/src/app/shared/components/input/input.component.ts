import { NgFor, NgIf } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LookupItem } from '../../api/lookup-item';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [NgIf, NgFor, ReactiveFormsModule],
  templateUrl: './input.component.html',
})
export class InputComponent {
  // Inputs
  @Input() label?: string;
  @Input() name?: string;
  @Input() autocomplete?: string;
  @Input() required?: boolean;
  @Input() disabled: boolean = false;
  @Input() readonly?: boolean;
  @Input() placeholder?: string | undefined;
  @Input() inputType?: 'text' | 'number' | 'password';
  @Input() inputClass?: string = '';
  @Input() parentGroup?: FormGroup;

  @Input() controlName?: string;

  @Input() dropdownVisible = false;
  @Input() suggestions: LookupItem[] = [];
  @Input() suggestionShowProperty: string = 'key';

  @Input('suggestions') set suggestionsSetter(arr: LookupItem[]) {
    this.suggestions = arr;
    this.dropdownVisible = !!arr.length;
  }

  currentValueOf(item: {}, selector: string): any {
    return item[selector as keyof typeof item];
  }

  @Output() onSuggestionClick = new EventEmitter<LookupItem>();

  onSuggestionClicked(item: LookupItem) {
    this.onSuggestionClick.emit(item);
  }
}
