import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({
  standalone:true,
  imports:[CommonModule,FormsModule,RouterLink],
  template:`
  <main class="page">
    <section class="visual">
      <div class="brand"><img src="/icons/signbridge-logo.svg" alt="">SignBridge</div>
      <div class="copy">
        <span>CREATE YOUR ACCOUNT</span>
        <h1>Start with one sign.<br>Grow from there.</h1>
        <p>Parents and children can create accounts with email. Teacher and Admin accounts are managed by the platform.</p>
      </div>
    </section>

    <section class="form-side">
      <form (ngSubmit)="submit()">
        <a routerLink="/login" class="back">← Back to sign in</a>
        <h2>Create account</h2>
        <p class="muted">Use your own email address.</p>

        <label>Full name</label>
        <input class="input" [(ngModel)]="fullName" name="fullName" autocomplete="name">

        <label>Email</label>
        <input class="input" [(ngModel)]="email" name="email" type="email" autocomplete="email">

        <label>Account type</label>
        <div class="role-grid">
          <button type="button" [class.active]="role===4" (click)="role=4">Child</button>
          <button type="button" [class.active]="role===3" (click)="role=3">Parent</button>
        </div>

        <label *ngIf="role===4">Date of birth <span>(optional)</span></label>
        <input *ngIf="role===4" class="input" [(ngModel)]="dateOfBirth" name="dateOfBirth" type="date">

        <label>Password</label>
        <input class="input" [(ngModel)]="password" name="password" type="password" autocomplete="new-password">

        <label>Confirm password</label>
        <input class="input" [(ngModel)]="confirmPassword" name="confirmPassword" type="password" autocomplete="new-password">

        <p class="error" *ngIf="error">{{error}}</p>
        <button class="submit" [disabled]="loading">{{loading?'Creating account...':'Create account'}}</button>
      </form>
    </section>
  </main>`,
  styles:[`
    .page{min-height:100vh;display:grid;grid-template-columns:1fr 520px;background:#f7f2e8}
    .visual{background:#173b3f;color:#fff;padding:48px;display:flex;flex-direction:column}
    .brand{display:flex;align-items:center;gap:12px;font:700 27px Georgia,serif}.brand img{width:46px;height:46px}
    .copy{margin:auto 0;max-width:650px}.copy span{font-size:11px;letter-spacing:.18em;color:#f6c96b;font-weight:900}.copy h1{font:600 clamp(50px,6vw,80px)/.95 Georgia,serif;letter-spacing:-3px;margin:18px 0}.copy p{font-size:17px;line-height:1.7;color:#d6e7e5;max-width:590px}
    .form-side{display:grid;place-items:center;padding:34px}.form-side form{width:min(410px,100%)}.back{font-size:13px;color:#66716d}.form-side h2{font:600 38px Georgia,serif;margin:22px 0 5px}
    label{display:block;margin:17px 0 7px;font-size:13px;font-weight:800}label span{font-weight:500;color:#81837f}.input{background:#fffdf8}
    .role-grid{display:grid;grid-template-columns:1fr 1fr;gap:9px}.role-grid button{border:1px solid #d8d2c5;background:#fffdf8;border-radius:11px;padding:12px;font-weight:800}.role-grid button.active{background:#173b3f;color:white;border-color:#173b3f}
    .submit{width:100%;border:0;border-radius:999px;background:#173b3f;color:white;padding:14px;font-weight:900;margin-top:22px}.error{background:#fff0ed;color:#ae4747;padding:10px;border-radius:10px}
    @media(max-width:900px){.page{grid-template-columns:1fr}.visual{display:none}}
  `]
})
export class RegisterComponent{
  fullName='';
  email='';
  role=4;
  dateOfBirth='';
  password='';
  confirmPassword='';
  error='';
  loading=false;

  constructor(private auth:AuthService){}

  submit(){
    this.error='';
    if(!this.fullName.trim() || !this.email.trim()){
      this.error='Full name and email are required.';
      return;
    }
    if(this.password.length<8){
      this.error='Password must be at least 8 characters.';
      return;
    }
    if(this.password!==this.confirmPassword){
      this.error='Passwords do not match.';
      return;
    }

    this.loading=true;
    this.auth.register({
      fullName:this.fullName.trim(),
      email:this.email.trim(),
      password:this.password,
      role:this.role,
      dateOfBirth:this.role===4 && this.dateOfBirth ? this.dateOfBirth : null
    }).subscribe({
      next:()=>this.auth.goHome(),
      error:e=>{
        this.loading=false;
        this.error=e?.error?.detail ?? 'Could not create account.';
      }
    });
  }
}
