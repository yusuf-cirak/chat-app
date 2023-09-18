import { NgClass, NgIf } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [NgIf, NgClass],
  templateUrl: './button.component.html',
})
export class ButtonComponent {
  @Input() label?: string;
  @Input() buttonType?: 'button' | 'submit' | 'reset' = 'button';
  @Input() disabled?: boolean;
  @Input() showIconAndHideLabelWhenDisabled?: boolean;
  @Input() class?: string;
}
