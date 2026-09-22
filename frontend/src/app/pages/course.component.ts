import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../core/api.service';
import { CourseDetails } from '../models';
import { ShellComponent } from '../shared/shell.component';

@Component({
  standalone:true,
  imports:[CommonModule,RouterLink,ShellComponent],
  template:`
    <app-shell title="Course" [nav]="nav">
      <div *ngIf="course" class="course-head card">
        <img class="cover" [src]="course.coverImageUrl || '/course-covers/first-signs.svg'" [alt]="course.title">
        <div>
          <a routerLink="/child" class="muted">← Back to courses</a>
          <h1>{{course.title}}</h1>
          <p>{{course.description}}</p>
        </div>
      </div>

      <section *ngFor="let level of course?.levels" class="level">
        <div class="level-title">
          <span class="pill">Level {{level.order}}</span>
          <h2>{{level.title}}</h2>
        </div>

        <div class="lessons">
          <a class="card lesson"
             *ngFor="let lesson of level.lessons"
             [class.locked]="!lesson.isUnlocked"
             [routerLink]="lesson.isUnlocked ? ['/child/lesson',lesson.id] : null">
            <div class="number">{{lesson.order}}</div>
            <div class="lesson-info">
              <h3>{{lesson.title}}</h3>
              <span class="muted">{{lesson.durationMinutes}} min · best score {{lesson.bestScore}}%</span>
            </div>
            <div class="status">
              <span *ngIf="lesson.isCompleted" class="done">✓ Completed</span>
              <span *ngIf="lesson.isUnlocked && !lesson.isCompleted">Start →</span>
              <span *ngIf="!lesson.isUnlocked">Locked</span>
            </div>
          </a>
        </div>
      </section>
    </app-shell>`,
  styles:[`
    .course-head{padding:22px;display:flex;gap:24px;align-items:center}.cover{width:210px;aspect-ratio:1.9/1;object-fit:cover;border-radius:16px}.course-head h1{margin:8px 0 6px;font-size:34px}.course-head p{margin:0;color:#6e7693;max-width:700px;line-height:1.6}
    .level{margin-top:28px}.level-title{margin-bottom:14px}.level-title h2{margin:8px 0 0}.lessons{display:grid;gap:12px}.lesson{display:flex;align-items:center;padding:18px;gap:16px}
    .number{width:44px;height:44px;border-radius:13px;background:#edf3ef;color:#173b3f;display:grid;place-items:center;font-weight:900}.lesson-info{flex:1}.lesson-info h3{margin:0 0 4px}.status{font-weight:800;color:#1f6f78}.done{color:#28765b}.locked{opacity:.48;cursor:not-allowed}
    @media(max-width:760px){.course-head{align-items:flex-start;flex-direction:column}.cover{width:100%}}
  `]
})
export class CourseComponent implements OnInit{
  course?:CourseDetails;
  nav=[
    {label:'Learning',link:'/child',icon:'▦'},
    {label:'Dictionary',link:'/child/dictionary',icon:'A'}
  ];

  constructor(private route:ActivatedRoute,private api:ApiService){}

  ngOnInit(){
    const id=this.route.snapshot.paramMap.get('id')!;
    this.api.getCourse(id).subscribe(course=>this.course=course);
  }
}
