import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({
  standalone:true,
  imports:[CommonModule,FormsModule,RouterLink],
  template:`
  <div class="page">
    <section class="visual">
      <div class="overlay">
        <div class="brand"><img src="/icons/signbridge-logo.svg" alt=""><span>SignBridge</span></div>
        <div class="copy">
          <span>LEARN VISUALLY · CONNECT NATURALLY</span>
          <h1>Small signs.<br>Big conversations.</h1>
          <p>A playful space where children, families and teachers build sign-language skills together.</p>
        </div>
        <small>Visual-first learning for everyday communication.</small>
      </div>
    </section>

    <section class="login">
      <form (ngSubmit)="submit()">
        <img class="seal" src="/icons/signbridge-logo.svg" alt="SignBridge">
        <span class="eyebrow">WELCOME BACK</span>
        <h2>Sign in with email</h2>
        <p class="muted">Use the email address connected to your SignBridge account.</p>

        <label>Email</label>
        <input class="input" [(ngModel)]="email" name="email" type="email" autocomplete="email" placeholder="name@example.com">

        <label>Password</label>
        <input class="input" [(ngModel)]="password" name="password" type="password" autocomplete="current-password">

        <p class="error" *ngIf="error">{{error}}</p>
        <button class="btn signin" [disabled]="loading">{{loading?'Signing in...':'Sign in'}}</button>

        <div class="create">
          <span>New to SignBridge?</span>
          <a routerLink="/register">Create an account</a>
        </div>


      </form>
    </section>
  </div>`,
  styles:[`
    .page{min-height:100vh;display:grid;grid-template-columns:minmax(0,1.4fr) minmax(390px,.6fr);background:#f7f2e8}
    .visual{background:#10183a url('/assets/signbridge-hero.png') center/cover no-repeat;min-height:100vh}.overlay{min-height:100vh;padding:42px 48px;color:white;display:flex;flex-direction:column;background:linear-gradient(90deg,rgba(9,14,42,.03),rgba(9,14,42,.18))}
    .brand{display:flex;gap:12px;align-items:center;font:700 28px Georgia,serif}.brand img{width:48px;height:48px}
    .copy{margin-top:auto;margin-bottom:70px;max-width:640px}.copy span{font-size:12px;letter-spacing:.17em;font-weight:900;color:#f4c46c}.copy h1{font:600 clamp(56px,6vw,90px)/.9 Georgia,serif;letter-spacing:-4px;margin:18px 0}.copy p{color:#e4e8ff;font-size:17px;line-height:1.7}.overlay small{color:#cbd0ee;margin-top:auto}
    .login{display:grid;place-items:center;padding:34px}.login form{width:min(430px,100%)}.seal{width:50px;height:50px;margin-bottom:26px}.eyebrow{font-size:11px;letter-spacing:.17em;color:#7d765f;font-weight:900}.login h2{font:600 38px Georgia,serif;letter-spacing:-1px;margin:7px 0 8px}
    label{display:block;font-size:13px;font-weight:800;margin:18px 0 7px}.input{background:#fffdf7;border-color:#ddd6c8}.signin{width:100%;margin-top:22px;border-radius:999px;background:#173b3f;color:white;padding:13px}.error{background:#fff0ed;color:#af4747;padding:10px;border-radius:10px}
    .create{display:flex;justify-content:space-between;align-items:center;margin-top:24px;padding-top:19px;border-top:1px solid #ded8ca;font-size:14px}.create a{font-weight:900;color:#173b3f}

    @media(max-width:900px){.page{grid-template-columns:1fr}.visual{min-height:340px}.overlay{min-height:340px;padding:26px}.copy{margin-top:auto;margin-bottom:10px}.copy h1{font-size:48px}.overlay small{display:none}}
  `]
})
export class LoginComponent{
  email='';
  password='';
  loading=false;
  error='';

  constructor(private auth:AuthService){}

  submit(){
    this.loading=true;
    this.error='';
    this.auth.login(this.email,this.password).subscribe({
      next:()=>this.auth.goHome(),
      error:e=>{
        this.loading=false;
        this.error=e?.error?.detail??'Invalid email or password.';
      }
    });
  }
}
