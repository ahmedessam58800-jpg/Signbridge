import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({
 selector:'app-shell',
 standalone:true,
 imports:[CommonModule,RouterLink,RouterLinkActive],
 template:`
 <div class="layout">
   <aside class="sidebar">
     <a class="brand" [routerLink]="homeLink" aria-label="SignBridge home">
       <img src="/icons/signbridge-logo.svg" alt="">
       <span><b>Sign</b>Bridge</span>
     </a>

     <nav class="nav" aria-label="Main navigation">
       <a
         *ngFor="let item of nav"
         [routerLink]="item.link"
         routerLinkActive="active"
         [routerLinkActiveOptions]="{exact:item.exact ?? false}">
         <span class="nav-icon" aria-hidden="true">{{item.icon}}</span>
         <span>{{item.label}}</span>
       </a>
     </nav>

     <div class="a11y">
       <button (click)="toggleLarge()" title="Toggle large text" aria-label="Toggle large text">Aa</button>
       <button (click)="toggleContrast()" title="Toggle high contrast" aria-label="Toggle high contrast">◐</button>
     </div>

     <div class="profile">
       <div class="avatar">{{initials}}</div>
       <div class="profile-text">
         <strong>{{auth.session?.fullName}}</strong>
         <small>{{auth.session?.role}}</small>
       </div>
     </div>

     <button class="logout" (click)="auth.logout()">Log out</button>
   </aside>

   <main>
     <header class="topbar">
       <div>
         <span class="muted kicker">Learn visually · communicate confidently</span>
         <h2>{{title}}</h2>
       </div>
       <span class="pill desktop-pill">Accessible learning</span>
     </header>

     <section class="content">
       <ng-content></ng-content>
     </section>
   </main>
 </div>`,
 styles:[`
 .layout{min-height:100vh;display:grid;grid-template-columns:250px 1fr}
 .sidebar{
   background:#173b3f;color:#fff;padding:26px 18px;display:flex;flex-direction:column;gap:22px;
   position:sticky;top:0;height:100vh
 }
 .brand{display:flex;align-items:center;gap:11px;font-size:22px}.brand img{width:43px;height:43px}.brand b{color:#f6c96b}
 .nav{display:grid;gap:7px}
 .nav a{
   display:flex;align-items:center;gap:10px;padding:12px 13px;border-radius:12px;
   color:#dceced;font-weight:750;transition:background .15s ease,color .15s ease
 }
 .nav a:hover,.nav a.active{background:#24535a;color:white}
 .nav-icon{width:24px;text-align:center;font-size:13px;font-weight:900;opacity:.9}
 .a11y{display:flex;gap:8px}
 .a11y button{border:1px solid #38656a;background:#214a50;color:#fff;width:42px;height:36px;border-radius:10px}
 .profile{margin-top:auto;display:flex;gap:10px;align-items:center;min-width:0}
 .avatar{width:42px;height:42px;flex:0 0 42px;border-radius:50%;background:#2a5c62;display:grid;place-items:center;font-weight:900}
 .profile-text{min-width:0}.profile strong{display:block;overflow:hidden;text-overflow:ellipsis;white-space:nowrap}
 .profile small{display:block;color:#a9cacc;margin-top:2px}
 .logout{border:1px solid #38656a;background:transparent;color:#fff;padding:10px;border-radius:10px;font-weight:800}
 main{min-width:0}
 .topbar{
   min-height:92px;background:#fff;border-bottom:1px solid #e6ebe9;padding:18px 28px;
   display:flex;align-items:center;justify-content:space-between;position:sticky;top:0;z-index:10
 }
 .topbar h2{margin:4px 0 0;font-size:24px}.kicker{font-size:12px}
 .content{padding:28px;max-width:1500px;margin:0 auto}
 @media(max-width:820px){
   .layout{grid-template-columns:1fr}.sidebar{display:none}.topbar{padding:16px 18px;position:static}.content{padding:18px}
   .desktop-pill{display:none}
 }
 `]
})
export class ShellComponent{
 @Input() title='';
 @Input() nav:{label:string;link:string;icon:string;exact?:boolean}[]=[];

 constructor(public auth:AuthService){}

 get initials(){
   return(this.auth.session?.fullName??'SB')
     .split(' ')
     .map(x=>x[0])
     .join('')
     .slice(0,2)
     .toUpperCase()
 }

 get homeLink(){
   return this.auth.role==='Child'?'/child':
     this.auth.role==='Parent'?'/parent':
     this.auth.role==='Teacher'?'/teacher':'/admin'
 }

 toggleLarge(){
   document.body.classList.toggle('a11y-large');
   localStorage.setItem('a11y-large',document.body.classList.contains('a11y-large')?'1':'0')
 }

 toggleContrast(){
   document.body.classList.toggle('a11y-contrast');
   localStorage.setItem('a11y-contrast',document.body.classList.contains('a11y-contrast')?'1':'0')
 }
}
