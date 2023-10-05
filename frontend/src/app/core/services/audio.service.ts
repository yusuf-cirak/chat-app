import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AudioService {
  private _audio = new Audio();

  setAudio() {
    this._audio.src = '../../../assets/audio/notification.mp3';
  }

  playNewMessageAudio() {
    this._audio.play();
  }
}
