import { CommonModule } from '@angular/common';
import { Component,OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../core/api.service';
import { ShellComponent } from '../shared/shell.component';

@Component({
  standalone:true,
  imports:[CommonModule,FormsModule,ShellComponent],
  template:`
  <app-shell title="Admin dashboard" [nav]="nav">
    <section class="card state" *ngIf="loading">
      <div class="state-inner"><div class="spinner"></div><strong>Loading platform overview…</strong></div>
    </section>

    <section class="card state" *ngIf="!loading && error">
      <div class="state-inner">
        <div class="state-icon">!</div>
        <strong>Could not load the dashboard</strong>
        <span class="muted">{{error}}</span>
        <button class="btn btn-primary" (click)="load()">Try again</button>
      </div>
    </section>

    <ng-container *ngIf="!loading && !error">
      <div class="grid grid-3" *ngIf="stats">
        <div class="card stat"><span>Total users</span><strong>{{stats.users}}</strong></div>
        <div class="card stat"><span>Children / Parents</span><strong>{{stats.children}} / {{stats.parents}}</strong></div>
        <div class="card stat"><span>Teachers</span><strong>{{stats.teachers}}</strong></div>
        <div class="card stat"><span>Courses live</span><strong>{{stats.publishedCourses}} / {{stats.courses}}</strong></div>
        <div class="card stat"><span>Completed lessons</span><strong>{{stats.completedLessons}}</strong></div>
        <div class="card stat"><span>Quiz attempts</span><strong>{{stats.quizAttempts}}</strong></div>
      </div>

      <section class="card users">
        <div class="users-head">
          <div><h2>User management</h2><p class="muted">Search accounts and control access.</p></div>
          <div class="filters">
            <input class="input" [(ngModel)]="search" (ngModelChange)="loadUsers()" placeholder="Search name or email">
            <select class="input" [(ngModel)]="role" (ngModelChange)="loadUsers()">
              <option value="">All roles</option><option>Child</option><option>Parent</option><option>Teacher</option><option>Admin</option>
            </select>
          </div>
        </div>

        <div class="table" *ngIf="users.length">
          <div class="row header"><span>Name</span><span>Email</span><span>Role</span><span>XP</span><span>Status</span></div>
          <div class="row" *ngFor="let u of users">
            <span><strong>{{u.fullName}}</strong></span>
            <span class="muted">{{u.email}}</span>
            <span><span class="pill">{{u.role}}</span></span>
            <span>{{u.totalXp}}</span>
            <span>
              <button class="status" [class.off]="!u.isActive" [disabled]="u.role==='Admin'" (click)="toggle(u)">
                {{u.isActive?'Active':'Disabled'}}
              </button>
            </span>
          </div>
        </div>

        <div class="empty" *ngIf="!users.length">No users match your current filters.</div>
      </section>
    </ng-container>
  </app-shell>`,
  styles:[`
    .stat{padding:22px}.stat span{color:#6b7d7c}.stat strong{display:block;margin-top:8px;font-size:30px}
    .users{margin-top:24px;padding:24px;overflow:auto}.users-head{display:flex;align-items:end;justify-content:space-between;gap:20px}.users-head h2{margin:0}
    .filters{display:flex;gap:8px}.filters .input:first-child{width:260px}.table{min-width:800px;margin-top:18px}
    .row{display:grid;grid-template-columns:1.1fr 1.4fr .7fr .4fr .6fr;gap:14px;padding:14px 8px;border-top:1px solid #e7e9e3;align-items:center}
    .row.header{color:#6b7d7c;font-size:12px;font-weight:900;text-transform:uppercase}
    .status{border:0;background:#e5f3ed;color:#257056;padding:7px 10px;border-radius:99px;font-weight:800}.status.off{background:#fae8e8;color:#aa3d3d}
    @media(max-width:900px){.users-head{display:block}.filters{margin-top:14px;flex-direction:column}.filters .input:first-child{width:100%}}
  `]
})
export class AdminDashboardComponent implements OnInit{
  stats:any;
  users:any[]=[];
  search='';
  role='';
  loading=true;
  error='';

  nav=[
    {label:'Overview',link:'/admin',icon:'01',exact:true},
    {label:'Content studio',link:'/teacher',icon:'02'}
  ];

  constructor(private api:ApiService){}

  ngOnInit(){this.load()}

  load(){
    this.loading=true;
    this.error='';

    this.api.getAdminDashboard().subscribe({
      next:x=>{
        this.stats=x;
        this.loadUsers();
      },
      error:e=>{
        this.loading=false;
        this.error=e?.error?.detail??'Please check that the API is running.';
      }
    });
  }

  loadUsers(){
    this.api.getUsers(this.search,this.role).subscribe({
      next:x=>{this.users=x;this.loading=false},
      error:e=>{
        this.loading=false;
        this.error=e?.error?.detail??'Could not load users.';
      }
    })
  }

  toggle(u:any){
    this.api.setUserActive(u.id,!u.isActive).subscribe(()=>this.loadUsers())
  }
}
