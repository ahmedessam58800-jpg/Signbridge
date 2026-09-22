import { CommonModule } from '@angular/common';
import { Component,OnInit } from '@angular/core';
import { ApiService } from '../core/api.service';
import { AuthService } from '../core/auth.service';
import { ChildProfile } from '../models';
import { ShellComponent } from '../shared/shell.component';

@Component({
  standalone:true,
  imports:[CommonModule,ShellComponent],
  template:`
  <app-shell title="My profile" [nav]="nav">
    <section class="card profile-head">
      <div class="avatar">{{initials}}</div>

      <div class="identity">
        <span class="pill">{{profile?.levelName || 'Child learner'}}</span>
        <h1>{{auth.session?.fullName}}</h1>
        <p>{{auth.session?.email}}</p>
        <small *ngIf="auth.session?.linkCode">Parent link code: <b>{{auth.session?.linkCode}}</b></small>
      </div>

      <div class="xp">
        <strong>{{profile?.totalXp || 0}}</strong>
        <span>XP</span>
      </div>
    </section>

    <div class="grid grid-3 stats">
      <div class="card stat"><span>Completed lessons</span><strong>{{profile?.completedLessons || 0}}</strong></div>
      <div class="card stat"><span>Quiz average</span><strong>{{profile?.averageQuizScore || 0}}%</strong></div>
      <div class="card stat"><span>Current streak</span><strong>{{profile?.currentStreak || 0}} days</strong></div>
    </div>

    <section class="card account">
      <h2>Account</h2>
      <div class="account-row"><span>Full name</span><strong>{{auth.session?.fullName}}</strong></div>
      <div class="account-row"><span>Email</span><strong>{{auth.session?.email}}</strong></div>
      <div class="account-row"><span>Account type</span><strong>{{auth.session?.role}}</strong></div>
    </section>

    <h2 class="section-heading">Achievements</h2>
    <div class="grid grid-3" *ngIf="profile">
      <article class="card badge" *ngFor="let a of profile.achievements" [class.locked]="!a.unlocked">
        <div>{{a.icon}}</div>
        <h3>{{a.title}}</h3>
        <p>{{a.description}}</p>
        <b>{{a.unlocked?'Unlocked':'Keep learning'}}</b>
      </article>
    </div>
  </app-shell>`,
  styles:[`
    .profile-head{padding:28px;display:flex;gap:20px;align-items:center}.avatar{width:92px;height:92px;border-radius:26px;background:#173b3f;color:#fff;display:grid;place-items:center;font-size:30px;font-weight:900}
    .identity h1{margin:9px 0 2px}.identity p{margin:0;color:#6b7d7c}.identity small{display:block;margin-top:8px;color:#6b7d7c}.xp{margin-left:auto;text-align:center;background:#f6c96b;color:#173b3f;padding:18px 25px;border-radius:18px}.xp strong{display:block;font-size:31px}
    .stats{margin:20px 0}.stat{padding:20px}.stat span{color:#6b7d7c}.stat strong{display:block;font-size:28px;margin-top:7px}.account{padding:23px}.account h2{margin-top:0}.account-row{display:flex;justify-content:space-between;gap:20px;padding:13px 0;border-top:1px solid #ece7dc}.account-row span{color:#6b7d7c}
    .section-heading{margin-top:28px}.badge{padding:22px}.badge>div{font-size:40px}.badge p{color:#6b7d7c;min-height:42px}.badge b{color:#1f6f78}.locked{opacity:.48;filter:grayscale(1)}
    @media(max-width:700px){.profile-head{align-items:flex-start;flex-wrap:wrap}.xp{margin-left:0}}
  `]
})
export class ProfileComponent implements OnInit{
  profile?:ChildProfile;

  nav=[
    {label:'Learning',link:'/child',icon:'▦'},
    {label:'Dictionary',link:'/child/dictionary',icon:'A'},
    {label:'Favorites',link:'/child/favorites',icon:'★'},
    {label:'Achievements',link:'/child/achievements',icon:'◆'},
    {label:'Profile',link:'/child/profile',icon:'○'}
  ];

  constructor(private api:ApiService,public auth:AuthService){}

  ngOnInit(){
    this.api.getProfile().subscribe(x=>this.profile=x);
  }

  get initials(){
    return (this.auth.session?.fullName??'SB')
      .split(' ')
      .map(x=>x[0])
      .join('')
      .slice(0,2)
      .toUpperCase();
  }
}
