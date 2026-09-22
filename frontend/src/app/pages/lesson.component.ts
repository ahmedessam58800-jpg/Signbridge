import { CommonModule } from '@angular/common';
import { Component,OnInit } from '@angular/core';
import { ActivatedRoute,RouterLink } from '@angular/router';
import { DomSanitizer,SafeResourceUrl } from '@angular/platform-browser';
import { ApiService } from '../core/api.service';
import { LessonDetails } from '../models';
import { ShellComponent } from '../shared/shell.component';

@Component({
  standalone:true,
  imports:[CommonModule,RouterLink,ShellComponent],
  template:`
  <app-shell title="Lesson" [nav]="nav">
    <div *ngIf="lesson" class="grid grid-2">
      <section>
        <div class="video card">
          <iframe
            *ngIf="youtubeUrl"
            [src]="youtubeUrl"
            title="Lesson video"
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
            allowfullscreen>
          </iframe>

          <video
            *ngIf="!youtubeUrl && lesson.videoUrl && lesson.videoUrl.startsWith('/')"
            controls
            [src]="api.absoluteMedia(lesson.videoUrl)">
          </video>

          <div class="placeholder" *ngIf="!youtubeUrl && (!lesson.videoUrl || !lesson.videoUrl.startsWith('/'))">
            <img src="/icons/signbridge-logo.svg" alt="">
            <strong>Visual lesson</strong>
            <small>No embedded video is available for this lesson yet.</small>
          </div>
        </div>

        <div class="card body">
          <div class="row">
            <span class="pill">{{lesson.durationMinutes}} min · {{lesson.xpReward}} XP</span>
            <button class="save" (click)="toggleFavorite()">{{lesson.isFavorite?'★ Saved':'☆ Save lesson'}}</button>
          </div>

          <h1>{{lesson.title}}</h1>
          <p>{{lesson.summary}}</p>

          <a
            *ngIf="lesson.videoUrl && isYoutube(lesson.videoUrl)"
            class="source-link"
            [href]="lesson.videoUrl"
            target="_blank"
            rel="noopener noreferrer">
            Open original video on YouTube ↗
          </a>

          <div class="caption">
            <b>Learning note</b>
            <span>
              Watch the visual demonstration first, then use the text summary and quiz.
              Sign languages are regional, so each course identifies the language or source used.
            </span>
          </div>
        </div>
      </section>

      <aside class="card side">
        <span class="pill">Your lesson</span>
        <h2>Progress</h2>
        <div class="metric"><span>Best score</span><strong>{{lesson.bestScore}}%</strong></div>
        <div class="metric"><span>Pass mark</span><strong>{{lesson.minimumPassingScore}}%</strong></div>
        <div class="metric"><span>Status</span><strong>{{lesson.isCompleted?'Completed':'In progress'}}</strong></div>
        <a class="btn btn-primary quiz-btn" [routerLink]="['/child/lesson',lesson.id,'quiz']">Take quiz</a>
      </aside>
    </div>
  </app-shell>`,
  styles:[`
    .video{padding:12px}.video iframe,.video video,.placeholder{width:100%;aspect-ratio:16/9;border:0;border-radius:13px}.video video{background:#111}
    .placeholder{background:#173b3f;color:#fff;display:grid;place-items:center;align-content:center;gap:8px}.placeholder img{width:68px;height:68px}.placeholder small{color:#b8d0d2}
    .body{padding:23px;margin-top:18px}.row{display:flex;justify-content:space-between;gap:12px}.save{border:0;background:#fff4d9;color:#9c6d15;padding:8px 11px;border-radius:10px;font-weight:800}
    .body h1{margin:14px 0 8px}.body p{color:#6b7d7c;line-height:1.7}.source-link{display:inline-block;margin-top:4px;color:#1f6f78;font-weight:850}
    .caption{margin-top:20px;background:#eef4f2;padding:15px;border-radius:12px}.caption span{display:block;color:#5f7372;margin-top:5px;line-height:1.55}
    .side{padding:24px;align-self:start}.metric{display:flex;justify-content:space-between;border-bottom:1px solid #e6ebe9;padding:16px 0}.metric span{color:#6b7d7c}.quiz-btn{display:block;text-align:center;margin-top:22px}
  `]
})
export class LessonComponent implements OnInit{
  lesson?:LessonDetails;
  youtubeUrl?:SafeResourceUrl;

  nav=[
    {label:'Learning',link:'/child',icon:'▦'},
    {label:'Dictionary',link:'/child/dictionary',icon:'A'},
    {label:'Favorites',link:'/child/favorites',icon:'★'},
    {label:'Profile',link:'/child/profile',icon:'○'}
  ];

  constructor(
    private route:ActivatedRoute,
    public api:ApiService,
    private sanitizer:DomSanitizer){}

  ngOnInit(){this.load()}

  load(){
    this.api.getLesson(this.route.snapshot.paramMap.get('id')!).subscribe(x=>{
      this.lesson=x;
      this.youtubeUrl=x.videoUrl && this.isYoutube(x.videoUrl)
        ? this.sanitizer.bypassSecurityTrustResourceUrl(this.toEmbedUrl(x.videoUrl))
        : undefined;
    });
  }

  isYoutube(url:string){
    return url.includes('youtube.com') || url.includes('youtu.be');
  }

  toEmbedUrl(url:string){
    let id='';
    let start='';

    try{
      const parsed=new URL(url);
      if(parsed.hostname.includes('youtu.be')){
        id=parsed.pathname.replace('/','');
      }else{
        id=parsed.searchParams.get('v') ?? '';
      }

      const time=parsed.searchParams.get('t');
      if(time){
        const seconds=this.parseTime(time);
        if(seconds>0)start=`?start=${seconds}`;
      }
    }catch{}

    return `https://www.youtube-nocookie.com/embed/${id}${start}`;
  }

  parseTime(value:string){
    if(/^\d+$/.test(value))return Number(value);
    const h=Number(value.match(/(\d+)h/)?.[1]??0);
    const m=Number(value.match(/(\d+)m/)?.[1]??0);
    const s=Number(value.match(/(\d+)s/)?.[1]??0);
    return h*3600+m*60+s;
  }

  toggleFavorite(){
    if(!this.lesson)return;
    const call=this.lesson.isFavorite
      ? this.api.removeFavorite(this.lesson.id)
      : this.api.addFavorite(this.lesson.id);
    call.subscribe(()=>this.load());
  }
}
