import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../core/api.service';
import { AuthService } from '../core/auth.service';
import { ShellComponent } from '../shared/shell.component';
import { SignEntry } from '../models';

@Component({
  standalone:true,
  imports:[CommonModule,FormsModule,ShellComponent],
  template:`
    <app-shell title="Sign dictionary" [nav]="nav">
      <section class="card search">
        <div>
          <span class="pill">Visual vocabulary</span>
          <h1>Find a word</h1>
          <p class="muted">Browse vocabulary used across SignBridge courses.</p>
        </div>

        <div class="filters">
          <input class="input" [(ngModel)]="q" (input)="load()" placeholder="Search English or Arabic...">
          <select class="input" [(ngModel)]="category" (change)="load()">
            <option value="">All categories</option>
            <option *ngFor="let c of categories" [value]="c">{{c}}</option>
          </select>
        </div>
      </section>

      <section class="card state" *ngIf="loading">
        <div class="state-inner"><div class="spinner"></div><strong>Loading dictionary…</strong></div>
      </section>

      <section class="card state" *ngIf="!loading && error">
        <div class="state-inner">
          <div class="state-icon">!</div>
          <strong>Dictionary unavailable</strong>
          <span class="muted">{{error}}</span>
          <button class="btn btn-primary" (click)="load()">Try again</button>
        </div>
      </section>

      <ng-container *ngIf="!loading && !error">
        <div class="summary">
          <strong>{{items.length}}</strong>
          <span class="muted">words found</span>
        </div>

        <div class="cards">
          <article class="card card-hover sign" *ngFor="let item of items">
            <div class="art">
              <img [src]="icon(item.category)" [alt]="item.category">
            </div>
            <div class="body">
              <span class="pill">{{item.category}}</span>
              <h2>{{item.word}}</h2>
              <div class="arabic" dir="rtl">{{item.arabicWord}}</div>
              <p class="muted">{{item.description}}</p>
              <span class="difficulty">{{item.difficulty}}</span>
            </div>
          </article>
        </div>

        <div class="card empty" *ngIf="!items.length">No matching signs found.</div>
      </ng-container>
    </app-shell>
  `,
  styles:[`
    .search{padding:24px;display:flex;justify-content:space-between;gap:20px;align-items:end}.search h1{margin:10px 0 4px}
    .filters{display:grid;grid-template-columns:1.4fr 1fr;gap:10px;width:min(560px,100%)}.summary{margin:18px 2px 10px;display:flex;gap:7px;align-items:baseline}.summary strong{font-size:25px}
    .cards{display:grid;grid-template-columns:repeat(4,minmax(0,1fr));gap:16px}.sign{overflow:hidden}.art{height:128px;display:grid;place-items:center;background:#f6f2e8}.art img{width:76px;height:76px}
    .body{padding:17px}.body h2{margin:11px 0 2px;font-size:20px}.arabic{font-size:20px;font-weight:800;color:#33403d}.body p{min-height:38px;font-size:13px;line-height:1.5}.difficulty{font-size:11px;font-weight:800;color:#8c7650;text-transform:uppercase;letter-spacing:.06em}
    .state{margin-top:18px}
    @media(max-width:1100px){.cards{grid-template-columns:repeat(3,1fr)}}
    @media(max-width:800px){.cards{grid-template-columns:1fr}.search{flex-direction:column;align-items:stretch}.filters{grid-template-columns:1fr}}
  `]
})
export class DictionaryComponent implements OnInit{
  q='';
  category='';
  items:SignEntry[]=[];
  loading=true;
  error='';
  categories=['Greetings','Family','Home','Food & Drink','School','Feelings','Needs','Colors','Numbers','Math','Time','Safety'];
  nav:any[]=[];

  constructor(private api:ApiService,private auth:AuthService){}

  ngOnInit(){
    this.nav=this.auth.role==='Child'
      ? [
          {label:'Learning',link:'/child',icon:'01'},
          {label:'Dictionary',link:'/child/dictionary',icon:'A'},
          {label:'Favorites',link:'/child/favorites',icon:'★'},
          {label:'Achievements',link:'/child/achievements',icon:'◆'},
          {label:'Profile',link:'/child/profile',icon:'○'}
        ]
      : [{label:'Dictionary',link:'/dictionary',icon:'A'}];

    this.load();
  }

  load(){
    this.loading=true;
    this.error='';

    this.api.getDictionary(this.q,this.category).subscribe({
      next:x=>{this.items=x;this.loading=false},
      error:e=>{
        this.loading=false;
        this.error=e?.error?.detail??'Please try again.';
      }
    });
  }

  icon(category:string){
    if(category==='Family' || category==='Home')return '/icons/family.svg';
    if(category==='Food & Drink')return '/icons/food.svg';
    if(category==='School')return '/icons/school.svg';
    if(category==='Feelings' || category==='Needs')return '/icons/feelings.svg';
    if(category==='Colors' || category==='Numbers' || category==='Math' || category==='Time')return '/icons/numbers.svg';
    if(category==='Safety')return '/icons/safety.svg';
    return '/icons/greetings.svg';
  }
}
