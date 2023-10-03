import {
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { LookupItem } from '../../api/lookup-item';
import { UserDto } from 'src/app/core/dtos/user-dto';

@Component({
  selector: 'app-chip-list',
  standalone: true,
  imports: [NgFor, NgIf],
  templateUrl: './chip-list.component.html',
})
export class ChipListComponent {
  @Input() chipItems: LookupItem[] = [];
  @Input() chipItemSelector: string = 'key';

  @Output() onChipRemoveClick = new EventEmitter<LookupItem>();

  removeChipItemClicked(index: number): void {
    this.onChipRemoveClick.emit(this.chipItems[index]);
  }

  displayChip(chip: LookupItem): string | number | UserDto {
    return chip[this.chipItemSelector as keyof LookupItem];
  }
}
