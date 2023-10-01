import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TokenService } from './core/services/token.service';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'ChatApp';

  private readonly tokenService = inject(TokenService);
  private readonly authService = inject(AuthService);

  ngOnInit() {
    const accessToken = this.tokenService.accesToken;
    if (accessToken) {
      this.tokenService.decodeAccessToken(accessToken);
      if (!this.tokenService.isAccessTokenExpired()) {
        this.authService.setUser(
          this.tokenService.getUserCredentialsFromDecodedToken()!
        );
      }
    }
  }
}
