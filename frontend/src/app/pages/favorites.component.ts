import { CommonModule } from '@angular/common';
import { Component,OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../core/api.service';
import { ShellComponent } from '../shared/shell.component';

@Component({
  standalone:true,
  imports:[CommonModule,RouterLink,ShellComponent],
  template:`
  <app-shell title="Favorite lessons" [nav]="nav">
    <section class="card state" *ngIf="loading">
      <div class="state-inner"><div class="spinner"></div><strong>Loading favorites…</strong></div>
    </section>

    <section class="card state" *ngIf="!loading && error">
      <div class="state-inner">
        <div class="state-icon">!</div>
        <strong>Could not load favorites</strong>
        <span class="muted">{{error}}</span>
        <button class="btn btn-primary" (click)="load()">Try again</button>
      </div>
    </section>

    <ng-container *ngIf="!loading && !error">
      <div class="grid grid-3" *ngIf="items.length">
        <a class="card card-hover item" *ngFor="let x of items" [routerLink]="['/child/lesson',x.id]">
          <span class="pill">{{x.courseTitle}}</span>
          <h3>{{x.title}}</h3>
          <p class="muted">{{x.levelTitle}}</p>
          <b>Open lesson →</b>
        </a>
      </div>

      <div class="card empty" *ngIf="!items.length">
        <strong>No favorite lessons yet.</strong>
        <p>Save a lesson while learning and it will appear here.</p>
      </div>
    </ng-container>
  </app-shell>`,
  styles:[`
    .item{padding:22px}.item h3{font-size:22px;margin:14px 0 5px}.item b{color:#1f6f78}
    .empty strong{display:block;color:#18373a;margin-bottom:7px}.empty p{margin:0}
  `]
})
export class FavoritesComponent implements OnInit{
  items:any[]=[];
  loading=true;
  error='';

  nav=[
    {label:'Learning',link:'/child',icon:'01'},
    {label:'Dictionary',link:'/child/dictionary',icon:'A'},
    {label:'Favorites',link:'/child/favorites',icon:'★'},
    {label:'Achievements',link:'/child/achievements',icon:'◆'},
    {label:'Profile',link:'/child/profile',icon:'○'}
  ];

  constructor(private api:ApiService){}

  ngOnInit(){this.load()}

  load(){
    this.loading=true;
    this.error='';
    this.api.getFavorites().subscribe({
      next:x=>{this.items=x;this.loading=false},
      error:e=>{this.loading=false;this.error=e?.error?.detail??'Please try again.'}
    });
  }
}
