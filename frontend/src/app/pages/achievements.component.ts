import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../core/api.service';
import { ShellComponent } from '../shared/shell.component';

@Component({
  standalone: true,
  imports: [CommonModule, ShellComponent],
  template: `
    <app-shell title="Achievements" [nav]="nav">
      <section class="card intro">
        <div>
          <span class="pill">Milestones</span>
          <h1>Keep building your bridge</h1>
          <p class="muted">Achievements unlock automatically as your XP grows.</p>
        </div>
        <div class="hero">◆</div>
      </section>

      <section class="card state" *ngIf="loading">
        <div class="state-inner"><div class="spinner"></div><strong>Loading achievements…</strong></div>
      </section>

      <section class="card state" *ngIf="!loading && error">
        <div class="state-inner">
          <div class="state-icon">!</div>
          <strong>Could not load achievements</strong>
          <span class="muted">{{error}}</span>
          <button class="btn btn-primary" (click)="load()">Try again</button>
        </div>
      </section>

      <div class="grid grid-2 badges" *ngIf="!loading && !error">
        <article class="card badge" *ngFor="let item of items" [class.locked]="!item.earned">
          <div class="icon">{{item.icon}}</div>
          <div>
            <span class="pill">{{item.requiredXp}} XP</span>
            <h2>{{item.name}}</h2>
            <p class="muted">{{item.description}}</p>
            <strong>{{item.earned?'Unlocked':'Keep learning'}}</strong>
          </div>
        </article>
      </div>
    </app-shell>
  `,
  styles:[`
    .intro{padding:26px;display:flex;justify-content:space-between;align-items:center}.intro h1{margin:10px 0 4px}.hero{font-size:54px;color:#f2b84b}
    .badges{margin-top:20px}.badge{padding:22px;display:flex;gap:18px;align-items:center}.badge.locked{opacity:.45;filter:grayscale(1)}
    .icon{width:74px;height:74px;border-radius:18px;display:grid;place-items:center;background:#f0eadc;font-size:38px}.badge h2{margin:10px 0 4px}
    .state{margin-top:20px}
  `]
})
export class AchievementsComponent implements OnInit{
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

  ngOnInit(){this.load();}

  load(){
    this.loading=true;
    this.error='';
    this.api.getAchievements().subscribe({
      next:x=>{this.items=x;this.loading=false},
      error:e=>{this.loading=false;this.error=e?.error?.detail??'Please try again.'}
    });
  }
}
