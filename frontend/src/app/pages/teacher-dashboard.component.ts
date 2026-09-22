import { CommonModule } from '@angular/common';
import { Component,OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../core/api.service';
import { ShellComponent } from '../shared/shell.component';

@Component({
  standalone:true,
  imports:[CommonModule,FormsModule,ShellComponent],
  template:`
  <app-shell title="Teacher studio" [nav]="nav">
    <div class="studio">
      <aside class="card sidebar-card">
        <div class="side-head">
          <div><span class="pill">CONTENT</span><h2>Courses</h2></div>
          <button class="btn btn-primary" (click)="startCourse()">+ New</button>
        </div>

        <button class="course-row" *ngFor="let c of courses" (click)="selectCourse(c.id)" [class.active]="course?.id===c.id">
          <div><strong>{{c.title}}</strong><span>{{c.levels}} levels · {{c.lessons}} lessons</span></div>
          <b>{{c.isPublished?'Live':'Draft'}}</b>
        </button>
      </aside>

      <main class="workspace">
        <section class="card panel" *ngIf="creatingCourse">
          <span class="pill">NEW COURSE</span>
          <h1>Create course</h1>
          <label>Title<input class="input" [(ngModel)]="courseForm.title"></label>
          <label>Description<textarea class="input" rows="4" [(ngModel)]="courseForm.description"></textarea></label>
          <label>Cover image URL<input class="input" [(ngModel)]="courseForm.coverImageUrl" placeholder="/course-covers/first-signs.svg"></label>
          <div class="actions"><button class="btn btn-primary" (click)="createCourse()">Create</button><button class="btn btn-soft" (click)="creatingCourse=false">Cancel</button></div>
        </section>

        <ng-container *ngIf="course && !creatingCourse">
          <section class="card panel top">
            <div><span class="pill">{{course.isPublished?'PUBLISHED':'DRAFT'}}</span><h1>{{course.title}}</h1><p class="muted">{{course.description}}</p></div>
            <div class="actions"><button class="btn btn-soft" (click)="publishCourse(!course.isPublished)">{{course.isPublished?'Unpublish':'Publish course'}}</button><button class="btn btn-primary" (click)="showLevel=!showLevel">+ Level</button></div>
          </section>

          <section class="card panel quick-form" *ngIf="showLevel">
            <input class="input" [(ngModel)]="levelTitle" placeholder="Level title">
            <input class="input" type="number" [(ngModel)]="levelOrder" placeholder="Order">
            <button class="btn btn-primary" (click)="createLevel()">Add level</button>
          </section>

          <section class="card panel" *ngFor="let level of course.levels">
            <div class="level-head">
              <div><span class="pill">LEVEL {{level.order}}</span><h2>{{level.title}}</h2></div>
              <button class="btn btn-soft" (click)="openLesson(level.id)">+ Lesson</button>
            </div>

            <div class="lesson-row" *ngFor="let lesson of level.lessons">
              <div>
                <strong>{{lesson.order}}. {{lesson.title}}</strong>
                <span>{{lesson.quiz ? lesson.quiz.questions.length + ' quiz questions' : 'No quiz yet'}}</span>
              </div>
              <div class="row-actions">
                <button (click)="publishLesson(lesson.id,!lesson.isPublished)">{{lesson.isPublished?'Unpublish':'Publish'}}</button>
                <button *ngIf="!lesson.quiz" (click)="openQuiz(lesson.id,lesson.title)">+ Quiz</button>
                <button *ngIf="lesson.quiz" (click)="openQuestion(lesson.quiz.id,lesson.title)">+ Question</button>
              </div>
            </div>
          </section>
        </ng-container>

        <section class="card panel" *ngIf="lessonLevelId">
          <h2>Add lesson</h2>
          <div class="two">
            <label>Title<input class="input" [(ngModel)]="lessonForm.title"></label>
            <label>Order<input class="input" type="number" [(ngModel)]="lessonForm.order"></label>
            <label>Duration (minutes)<input class="input" type="number" [(ngModel)]="lessonForm.durationMinutes"></label>
            <label>XP reward<input class="input" type="number" [(ngModel)]="lessonForm.xpReward"></label>
          </div>
          <label>Summary<textarea class="input" rows="3" [(ngModel)]="lessonForm.summary"></textarea></label>
          <label>Video URL <span class="hint">Use only verified sign-language content.</span><input class="input" [(ngModel)]="lessonForm.videoUrl"></label>
          <div class="actions"><button class="btn btn-primary" (click)="createLesson()">Save lesson</button><button class="btn btn-soft" (click)="lessonLevelId=''">Cancel</button></div>
        </section>

        <section class="card panel" *ngIf="quizLessonId">
          <h2>Create quiz for {{quizLessonTitle}}</h2>
          <label>Quiz title<input class="input" [(ngModel)]="quizTitle"></label>
          <div class="actions"><button class="btn btn-primary" (click)="createQuiz()">Create quiz</button><button class="btn btn-soft" (click)="quizLessonId=''">Cancel</button></div>
        </section>

        <section class="card panel" *ngIf="questionQuizId">
          <h2>Add question to {{questionLessonTitle}}</h2>
          <label>Question<input class="input" [(ngModel)]="questionText"></label>
          <div class="two">
            <label>Correct answer<input class="input" [(ngModel)]="correctAnswer"></label>
            <label>Wrong answer 1<input class="input" [(ngModel)]="wrong1"></label>
            <label>Wrong answer 2<input class="input" [(ngModel)]="wrong2"></label>
            <label>Order<input class="input" type="number" [(ngModel)]="questionOrder"></label>
          </div>
          <div class="actions"><button class="btn btn-primary" (click)="createQuestion()">Add question</button><button class="btn btn-soft" (click)="questionQuizId=''">Cancel</button></div>
        </section>

        <div class="card empty" *ngIf="!course && !creatingCourse">Select a course or create a new one.</div>
      </main>
    </div>
  </app-shell>`,
  styles:[`
    .studio{display:grid;grid-template-columns:320px 1fr;gap:20px;align-items:start}.sidebar-card,.panel{padding:22px}.sidebar-card{position:sticky;top:20px}
    .side-head,.level-head,.top{display:flex;justify-content:space-between;align-items:center;gap:15px}.side-head h2,.level-head h2,.top h1{margin:8px 0}
    .course-row{width:100%;border:0;background:transparent;text-align:left;padding:14px;border-radius:12px;display:flex;justify-content:space-between;margin-top:8px}.course-row.active,.course-row:hover{background:#f0ece3}.course-row span{display:block;color:#757887;font-size:12px;margin-top:3px}.course-row b{font-size:11px;color:#173b3f}
    .workspace{display:grid;gap:16px}.actions{display:flex;gap:8px;flex-wrap:wrap}.quick-form{display:grid;grid-template-columns:1fr 100px auto;gap:9px}
    .lesson-row{display:flex;justify-content:space-between;gap:15px;padding:14px 0;border-top:1px solid #ece7dc}.lesson-row span{display:block;color:#757887;font-size:12px;margin-top:4px}.row-actions{display:flex;gap:6px;flex-wrap:wrap}.row-actions button{border:1px solid #d9d5ca;background:#fffdf8;border-radius:9px;padding:8px 9px;font-weight:750}
    label{display:grid;gap:7px;font-weight:800;margin:12px 0}.hint{font-weight:500;color:#888;font-size:11px}.two{display:grid;grid-template-columns:1fr 1fr;gap:12px}
    @media(max-width:960px){.studio{grid-template-columns:1fr}.sidebar-card{position:static}.quick-form,.two{grid-template-columns:1fr}}
  `]
})
export class TeacherDashboardComponent implements OnInit{
  nav=[{label:'Content studio',link:'/teacher',icon:'▦'},{label:'Dictionary',link:'/dictionary',icon:'A'}];
  courses:any[]=[];
  course:any=null;
  creatingCourse=false;
  showLevel=false;

  courseForm={title:'',description:'',coverImageUrl:'/course-covers/first-signs.svg'};
  levelTitle='';
  levelOrder=1;

  lessonLevelId='';
  lessonForm={title:'',summary:'',videoUrl:null as string|null,thumbnailUrl:null as string|null,durationMinutes:7,order:1,xpReward:50,minimumPassingScore:80};

  quizLessonId='';
  quizLessonTitle='';
  quizTitle='';

  questionQuizId='';
  questionLessonTitle='';
  questionText='';
  correctAnswer='';
  wrong1='';
  wrong2='';
  questionOrder=1;

  constructor(private api:ApiService){}

  ngOnInit(){this.loadCourses();}

  loadCourses(){this.api.getContentCourses().subscribe(x=>this.courses=x);}

  startCourse(){this.creatingCourse=true;this.course=null;}

  selectCourse(id:string){
    this.creatingCourse=false;
    this.api.getContentCourse(id).subscribe(x=>this.course=x);
  }

  createCourse(){
    this.api.createCourse(this.courseForm).subscribe(x=>{
      this.creatingCourse=false;
      this.loadCourses();
      this.selectCourse(x.id);
    });
  }

  publishCourse(value:boolean){
    this.api.publishCourse(this.course.id,value).subscribe(()=>this.selectCourse(this.course.id));
  }

  createLevel(){
    this.api.createLevel(this.course.id,{title:this.levelTitle,order:this.levelOrder}).subscribe(()=>{
      this.levelTitle='';
      this.levelOrder++;
      this.showLevel=false;
      this.selectCourse(this.course.id);
      this.loadCourses();
    });
  }

  openLesson(levelId:string){
    this.lessonLevelId=levelId;
    this.lessonForm={title:'',summary:'',videoUrl:null,thumbnailUrl:null,durationMinutes:7,order:1,xpReward:50,minimumPassingScore:80};
  }

  createLesson(){
    this.api.createLesson(this.lessonLevelId,this.lessonForm).subscribe(()=>{
      this.lessonLevelId='';
      this.selectCourse(this.course.id);
      this.loadCourses();
    });
  }

  publishLesson(id:string,value:boolean){
    this.api.publishLesson(id,value).subscribe(()=>this.selectCourse(this.course.id));
  }

  openQuiz(lessonId:string,title:string){
    this.quizLessonId=lessonId;
    this.quizLessonTitle=title;
    this.quizTitle=`${title} Quiz`;
  }

  createQuiz(){
    this.api.createQuiz(this.quizLessonId,this.quizTitle).subscribe(()=>{
      this.quizLessonId='';
      this.selectCourse(this.course.id);
    });
  }

  openQuestion(quizId:string,title:string){
    this.questionQuizId=quizId;
    this.questionLessonTitle=title;
    this.questionText='';
    this.correctAnswer='';
    this.wrong1='';
    this.wrong2='';
    this.questionOrder=1;
  }

  createQuestion(){
    this.api.createQuestion(this.questionQuizId,{
      text:this.questionText,
      order:this.questionOrder,
      answers:[
        {text:this.correctAnswer,isCorrect:true},
        {text:this.wrong1,isCorrect:false},
        {text:this.wrong2,isCorrect:false}
      ]
    }).subscribe(()=>{
      this.questionText='';
      this.correctAnswer='';
      this.wrong1='';
      this.wrong2='';
      this.questionOrder++;
      this.selectCourse(this.course.id);
    });
  }
}
