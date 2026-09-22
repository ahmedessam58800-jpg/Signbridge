import { CommonModule } from '@angular/common';
import { Component,OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../core/api.service';
import { AuthService } from '../core/auth.service';
import { CourseSummary,ChildProfile } from '../models';
import { ShellComponent } from '../shared/shell.component';

@Component({
  standalone:true,
  imports:[CommonModule,RouterLink,ShellComponent],
  template:`
  <app-shell title="My learning" [nav]="nav">

    <section class="card state" *ngIf="loading">
      <div class="state-inner">
        <div class="spinner"></div>
        <strong>Loading your learning space…</strong>
      </div>
    </section>

    <section class="card state" *ngIf="!loading && error">
      <div class="state-inner">
        <div class="state-icon">!</div>
        <strong>We could not load your courses.</strong>
        <span class="muted">{{error}}</span>
        <button class="btn btn-primary" (click)="load()">Try again</button>
      </div>
    </section>

    <ng-container *ngIf="!loading && !error">
      <section class="welcome card">
        <div>
          <span class="pill">Hi {{auth.session?.fullName}}</span>
          <h1>What will you learn today?</h1>
          <p>Continue a course, explore the dictionary, or work toward your next achievement.</p>
          <div class="quick">
            <a routerLink="/child/dictionary">Open dictionary</a>
            <a routerLink="/child/achievements">Achievements</a>
          </div>
        </div>
        <div class="streak"><small>Current streak</small><b>{{profile?.currentStreak??0}}</b><span>days</span></div>
      </section>

      <div class="stats grid grid-3">
        <div class="card stat"><span>Total XP</span><strong>{{profile?.totalXp??0}}</strong></div>
        <div class="card stat"><span>Lessons complete</span><strong>{{profile?.completedLessons??0}}</strong></div>
        <div class="card stat"><span>Quiz average</span><strong>{{profile?.averageQuizScore??0}}%</strong></div>
      </div>

      <div class="heading">
        <div>
          <h2>Course library</h2>
          <p class="muted">Video-backed courses for letters, ASL vocabulary, numbers and early math.</p>
        </div>
      </div>

      <div class="grid grid-3" *ngIf="courses.length">
        <a class="card card-hover course" *ngFor="let c of courses" [routerLink]="['/child/course',c.id]">
          <div class="art" [style.backgroundImage]="'url(' + (c.coverImageUrl || '/course-covers/first-signs.svg') + ')'">
            <b>{{c.progressPercent}}%</b>
          </div>
          <div class="body">
            <span class="pill">{{c.completedLessons}}/{{c.totalLessons}} lessons</span>
            <h3>{{c.title}}</h3>
            <p>{{c.description}}</p>
            <div class="progress"><span [style.width.%]="c.progressPercent"></span></div>
            <footer><strong>{{c.progressPercent}}% complete</strong><span>Open course →</span></footer>
          </div>
        </a>
      </div>

      <div class="card empty" *ngIf="!courses.length">
        No published courses are available yet.
      </div>
    </ng-container>
  </app-shell>`,
  styles:[`
    .welcome{padding:30px;display:flex;justify-content:space-between;align-items:center;background:#fff8e8;border-color:#eddfbb}
    .welcome h1{font-size:38px;margin:12px 0 6px}.welcome p{color:#6b7d7c;max-width:660px;line-height:1.55}
    .quick{display:flex;gap:10px;margin-top:18px;flex-wrap:wrap}
    .quick a{padding:10px 13px;border-radius:10px;background:#173b3f;color:#fff;font-weight:800}.quick a+a{background:#e6efed;color:#1f6f78}
    .streak{min-width:130px;text-align:center;background:#f2b84b;color:#173b3f;padding:20px;border-radius:24px}.streak small,.streak span{display:block}.streak b{font-size:44px}
    .stats{margin:20px 0 30px}.stat{padding:20px}.stat span{color:#6b7d7c}.stat strong{display:block;font-size:30px;margin-top:7px}
    .heading h2{margin-bottom:4px}
    .course{overflow:hidden}
    .art{height:165px;background-size:cover;background-position:center;display:flex;align-items:flex-start;justify-content:flex-end;padding:16px}
    .art b{background:rgba(255,255,255,.94);padding:7px 9px;border-radius:999px;color:#173b3f}
    .body{padding:20px}.body h3{font-size:22px;margin:13px 0 7px}.body p{color:#6b7d7c;min-height:66px;line-height:1.45}
    .body footer{display:flex;justify-content:space-between;gap:10px;margin-top:12px;font-size:13px}.body footer span{color:#1f6f78;font-weight:800}
    @media(max-width:700px){.welcome{align-items:flex-start;flex-direction:column;gap:20px}.streak{width:100%}.welcome h1{font-size:31px}}
  `]
})
export class ChildDashboardComponent implements OnInit{
  courses:CourseSummary[]=[];
  profile?:ChildProfile;
  loading=true;
  error='';

  nav=[
    {label:'Learning',link:'/child',icon:'01',exact:true},
    {label:'Dictionary',link:'/child/dictionary',icon:'A'},
    {label:'Favorites',link:'/child/favorites',icon:'★'},
    {label:'Achievements',link:'/child/achievements',icon:'◆'},
    {label:'Profile',link:'/child/profile',icon:'○'}
  ];

  constructor(public auth:AuthService,private api:ApiService){}

  ngOnInit(){this.load();}

  load(){
    this.loading=true;
    this.error='';

    this.api.getCourses().subscribe({
      next:x=>{
        this.courses=x;
        this.api.getProfile().subscribe({
          next:p=>{this.profile=p;this.loading=false},
          error:()=>{this.loading=false}
        });
      },
      error:e=>{
        this.loading=false;
        this.error=e?.error?.detail??'Please check that the API is running.';
      }
    });
  }
}
