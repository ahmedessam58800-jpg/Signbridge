import { CommonModule } from '@angular/common';
import { Component,OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../core/api.service';
import { ShellComponent } from '../shared/shell.component';

@Component({
  standalone:true,
  imports:[CommonModule,FormsModule,ShellComponent],
  template:`
  <app-shell title="Family progress" [nav]="nav">
    <section class="card link">
      <div>
        <span class="pill">Connect a child</span>
        <h3>Link using the child's email and private code</h3>
      </div>
      <input class="input" [(ngModel)]="childEmail" placeholder="child@email.com">
      <input class="input" [(ngModel)]="linkCode" placeholder="KID-123456">
      <button class="btn btn-primary" [disabled]="linking" (click)="link()">{{linking?'Linking…':'Link child'}}</button>
    </section>

    <p class="notice" [class.error]="noticeError" *ngIf="notice">{{notice}}</p>

    <section class="card state" *ngIf="loading">
      <div class="state-inner"><div class="spinner"></div><strong>Loading family progress…</strong></div>
    </section>

    <ng-container *ngIf="!loading">
      <div class="children grid grid-3" *ngIf="children.length">
        <button class="card card-hover child" *ngFor="let child of children" (click)="load(child.id)">
          <div class="avatar">{{initials(child.fullName)}}</div>
          <div>
            <strong>{{child.fullName}}</strong>
            <span>{{child.totalXp}} XP · {{child.currentStreak}} day streak</span>
          </div>
        </button>
      </div>

      <div class="card empty" *ngIf="!children.length">
        <strong>No child is linked yet.</strong>
        <p>Use the email and private link code above to connect an account.</p>
      </div>

      <section *ngIf="progress">
        <div class="card hero">
          <div>
            <span class="pill">Child progress</span>
            <h1>{{progress.childName}}</h1>
            <p class="muted">A clear overview of learning activity and results.</p>
          </div>
          <div class="ring">{{progress.progressPercent}}%</div>
        </div>

        <div class="grid grid-3 stats">
          <div class="card stat"><span>Completed lessons</span><strong>{{progress.completedLessons}}/{{progress.totalLessons}}</strong></div>
          <div class="card stat"><span>Average quiz score</span><strong>{{progress.averageQuizScore}}%</strong></div>
          <div class="card stat"><span>Total XP</span><strong>{{progress.totalXp}}</strong></div>
        </div>

        <div class="grid grid-2">
          <div class="card panel">
            <h2>Course progress</h2>
            <div class="course-row" *ngFor="let c of progress.courses">
              <div><strong>{{c.courseTitle}}</strong><span>{{c.completedLessons}}/{{c.totalLessons}} lessons</span></div>
              <b>{{c.progressPercent}}%</b>
            </div>
          </div>

          <div class="card panel">
            <h2>Recent completions</h2>
            <div class="recent" *ngFor="let item of progress.recentCompletions">
              <span>✓</span>
              <div><strong>{{item.lessonTitle}}</strong><small>Best score {{item.bestScore}}%</small></div>
            </div>
            <p class="muted" *ngIf="!progress.recentCompletions.length">No completed lessons yet.</p>
          </div>
        </div>
      </section>
    </ng-container>
  </app-shell>`,
  styles:[`
    .link{padding:18px;display:grid;grid-template-columns:1.2fr 1fr .7fr auto;gap:10px;align-items:end}.link h3{margin:7px 0 0}
    .notice{background:#e7f3ee;color:#216b53;padding:10px 13px;border-radius:10px}.notice.error{background:#faeaea;color:#a23f3f}
    .children{margin:22px 0}.child{border:1px solid #e3e5df;padding:18px;display:flex;gap:12px;text-align:left;align-items:center}
    .avatar{width:48px;height:48px;border-radius:16px;display:grid;place-items:center;background:#e8f1ef;color:#173b3f;font-weight:900}
    .child span{display:block;color:#6b7d7c;margin-top:4px;font-size:13px}
    .hero{padding:28px;display:flex;justify-content:space-between;align-items:center}.hero h1{margin:10px 0 6px}
    .ring{width:95px;height:95px;border-radius:50%;border:12px solid #f2b84b;display:grid;place-items:center;font-size:23px;font-weight:900}
    .stats{margin:20px 0}.stat,.panel{padding:22px}.stat span{color:#6b7d7c}.stat strong{display:block;font-size:28px;margin-top:8px}
    .course-row{display:flex;justify-content:space-between;padding:14px 0;border-bottom:1px solid #e7e9e3}.course-row span,.recent small{display:block;color:#6b7d7c;margin-top:4px;font-size:13px}
    .recent{display:flex;gap:12px;padding:12px 0}.recent>span{color:#2a8b68;font-weight:900}
    .empty strong{display:block;color:#18373a;margin-bottom:7px}.empty p{margin:0}
    @media(max-width:950px){.link{grid-template-columns:1fr}.hero{align-items:flex-start;gap:18px;flex-direction:column}}
  `]
})
export class ParentDashboardComponent implements OnInit{
  children:any[]=[];
  progress:any;
  childEmail='';
  linkCode='';
  notice='';
  noticeError=false;
  loading=true;
  linking=false;

  nav=[{label:'Family progress',link:'/parent',icon:'01',exact:true}];

  constructor(private api:ApiService){}

  ngOnInit(){this.refresh()}

  refresh(){
    this.loading=true;
    this.api.getChildren().subscribe({
      next:x=>{
        this.children=x;
        this.loading=false;
        if(x.length)this.load(x[0].id);
      },
      error:()=>{
        this.loading=false;
        this.notice='Could not load linked children.';
        this.noticeError=true;
      }
    })
  }

  load(id:string){
    this.api.getChildProgress(id).subscribe(x=>this.progress=x)
  }

  link(){
    this.notice='';
    this.noticeError=false;
    this.linking=true;

    this.api.linkChild(this.childEmail,this.linkCode).subscribe({
      next:()=>{
        this.linking=false;
        this.notice='Child linked successfully.';
        this.childEmail='';
        this.linkCode='';
        this.refresh();
      },
      error:e=>{
        this.linking=false;
        this.noticeError=true;
        this.notice=e?.error?.detail??'Could not link this child.';
      }
    })
  }

  initials(name:string){
    return name.split(' ').map((x:string)=>x[0]).join('').slice(0,2).toUpperCase()
  }
}
