
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  standalone: true,
  imports: [RouterLink],
  template: `
    <main style="padding:60px">
      <h1>SignBridge</h1>
      <p>Learn sign language visually.</p>
      <a routerLink="/login">Start learning</a>
    </main>
  `
})
export class LandingComponent {}
