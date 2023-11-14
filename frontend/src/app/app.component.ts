import { Component, inject } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { TokenService } from './core/services/token.service';
import { AuthService } from './core/services/auth.service';
import { filter } from 'rxjs';

declare var gtag: any;
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

  constructor(router: Router) {
    const navEvents = router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd)
    );
    navEvents.subscribe({
      next: (event: NavigationEnd) => {
        gtag('config', 'G-L4ZXTH0ZXK', {
          page_path: event.urlAfterRedirects,
        });
      },
    });
  }

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
