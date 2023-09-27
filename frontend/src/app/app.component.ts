import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TokenService } from './core/services/token.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'ChatApp';

  private readonly tokenService = inject(TokenService);

  ngOnInit() {
    const accessToken = this.tokenService.accesToken;
    if (accessToken) {
      this.tokenService.setUserCredentialsFromToken(accessToken);
    }
  }
}
