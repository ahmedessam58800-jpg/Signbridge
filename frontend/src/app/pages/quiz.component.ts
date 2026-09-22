import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../core/api.service';
import { Quiz } from '../models';
import { ShellComponent } from '../shared/shell.component';

@Component({
  standalone: true,
  imports: [CommonModule, ShellComponent],
  template: `
    <app-shell title="Quiz" [nav]="nav">
      <section class="card state" *ngIf="loading">
        <div class="state-inner"><div class="spinner"></div><strong>Preparing quiz…</strong></div>
      </section>

      <section class="card state" *ngIf="!loading && error">
        <div class="state-inner">
          <div class="state-icon">!</div>
          <strong>Quiz unavailable</strong>
          <span class="muted">{{error}}</span>
          <button class="btn btn-primary" (click)="load()">Try again</button>
        </div>
      </section>

      <div class="quiz-wrap" *ngIf="!loading && !error && quiz">
        <div class="card quiz-card" *ngIf="!result">
          <div class="quiz-progress">
            <span class="pill">Question {{ index + 1 }} of {{ quiz.questions.length }}</span>
            <div class="progress"><span [style.width.%]="((index+1)/quiz.questions.length)*100"></span></div>
          </div>

          <h1>{{ current.text }}</h1>

          <div class="answers">
            <button
              *ngFor="let answer of current.answers"
              [class.selected]="selected[current.id] === answer.id"
              (click)="selected[current.id] = answer.id">
              {{ answer.text }}
            </button>
          </div>

          <div class="actions">
            <button class="btn btn-soft" (click)="back()" [disabled]="index === 0 || submitting">Back</button>
            <button class="btn btn-primary" (click)="next()" [disabled]="!selected[current.id] || submitting">
              {{ submitting ? 'Submitting…' : (index === quiz.questions.length - 1 ? 'Submit quiz' : 'Next') }}
            </button>
          </div>
        </div>

        <div class="card result" *ngIf="result">
          <div class="result-icon">{{ result.passed ? '✓' : '↻' }}</div>
          <h1>{{ result.passed ? 'Great job!' : 'Keep practicing' }}</h1>
          <p>You scored <strong>{{ result.score }}%</strong></p>
          <p class="muted">{{ result.correctAnswers }} correct answers out of {{ result.totalQuestions }}</p>
          <p *ngIf="result.awardedXp">+{{ result.awardedXp }} XP earned</p>
          <button class="btn btn-primary" (click)="router.navigateByUrl('/child')">Back to learning</button>
        </div>
      </div>
    </app-shell>
  `,
  styles: [`
    .quiz-wrap{max-width:760px;margin:30px auto}.quiz-card,.result{padding:34px}
    .quiz-progress{display:grid;grid-template-columns:auto 1fr;align-items:center;gap:14px}.quiz-progress .progress{width:100%}
    .quiz-card h1{margin:24px 0 26px}.answers{display:grid;gap:12px}
    .answers button{text-align:left;border:2px solid #e8ece9;background:white;border-radius:14px;padding:16px;font-weight:800;color:#2b3150}
    .answers button:hover{border-color:#bfd4cf}.answers button.selected{border-color:#1f6f78;background:#edf6f4;color:#173b3f}
    .actions{margin-top:26px;display:flex;justify-content:space-between}.result{text-align:center}.result-icon{width:74px;height:74px;border-radius:50%;background:#e7f3ee;color:#257056;display:grid;place-items:center;font-size:38px;font-weight:900;margin:0 auto}
    .result h1{margin-bottom:8px}.result .btn{margin-top:16px}
  `]
})
export class QuizComponent implements OnInit {
  lessonId='';
  quiz?:Quiz;
  index=0;
  selected:Record<string,string>={};
  result:any;
  loading=true;
  submitting=false;
  error='';

  nav=[{label:'Learning',link:'/child',icon:'01'}];

  constructor(
    private route:ActivatedRoute,
    private api:ApiService,
    public router:Router){}

  ngOnInit(){
    this.lessonId=this.route.snapshot.paramMap.get('id')!;
    this.load();
  }

  load(){
    this.loading=true;
    this.error='';
    this.api.getQuiz(this.lessonId).subscribe({
      next:quiz=>{
        this.quiz=quiz;
        this.loading=false;
        if(!quiz.questions.length)this.error='This lesson does not have quiz questions yet.';
      },
      error:e=>{
        this.loading=false;
        this.error=e?.error?.detail??'Please try again.';
      }
    });
  }

  get current(){
    return this.quiz!.questions[this.index];
  }

  back(){
    if(this.index>0)this.index--;
  }

  next(){
    if(!this.quiz)return;

    if(this.index<this.quiz.questions.length-1){
      this.index++;
      return;
    }

    const answers=this.quiz.questions.map(q=>({
      questionId:q.id,
      answerId:this.selected[q.id]
    }));

    this.submitting=true;
    this.api.submitQuiz(this.lessonId,answers).subscribe({
      next:result=>{
        this.result=result;
        this.submitting=false;
      },
      error:e=>{
        this.submitting=false;
        this.error=e?.error?.detail??'Could not submit quiz.';
      }
    });
  }
}
