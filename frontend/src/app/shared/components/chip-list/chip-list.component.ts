import {
  Component,
  EventEmitter,
  Input,
  Output,
  WritableSignal,
  signal,
} from '@angular/core';
import { NgFor } from '@angular/common';
import { LookupItem } from '../../api/lookup-item';

@Component({
  selector: 'app-chip-list',
  standalone: true,
  imports: [NgFor],
  templateUrl: './chip-list.component.html',
})
export class ChipListComponent {
  @Input() chipItems: LookupItem[] = [];
  @Input() chipItemSelector: string = 'key';

  @Output() onChipRemoveClick = new EventEmitter<LookupItem>();

  removeChipItemClicked(index: number): void {
    this.onChipRemoveClick.emit(this.chipItems[index]);
  }

  displayChip(chip: LookupItem): string | number {
    return chip[this.chipItemSelector as keyof LookupItem];
  }
}
