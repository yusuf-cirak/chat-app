import { Injectable, inject } from '@angular/core';
import { HttpClientService } from 'src/app/shared/services/http-client.service';
import { UploadProfileImageDto } from '../dtos/upload-profile-image-dto';
import { Observable, map } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class ImageService {
  private readonly _httpClientService = inject(HttpClientService);

  uploadProfileImage(
    uploadProfileImageDto: UploadProfileImageDto
  ): Observable<string> {
    const formData = new FormData();
    formData.append('file', uploadProfileImageDto.file);
    formData.append('userId', uploadProfileImageDto.userId);
    return this._httpClientService.post(
      { controller: 'users', action: 'profile-image', responseType: 'text' },
      formData
    );
  }
}
