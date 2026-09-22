import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AuthResponse, CourseDetails, CourseSummary, LessonDetails, Quiz, SignEntry, ChildProfile } from '../models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  readonly baseUrl='http://localhost:5187/api';
  readonly serverUrl='http://localhost:5187';

  constructor(private http:HttpClient){}

  login(email:string,password:string){
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/login`,{email,password});
  }

  register(payload:{
    fullName:string;
    email:string;
    password:string;
    role:number;
    dateOfBirth:string|null;
  }){
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/register`,payload);
  }

  logout(refreshToken:string){
    return this.http.post<void>(`${this.baseUrl}/auth/logout`,{refreshToken});
  }

  getCourses(){ return this.http.get<CourseSummary[]>(`${this.baseUrl}/learning/courses`); }
  getCourse(id:string){ return this.http.get<CourseDetails>(`${this.baseUrl}/learning/courses/${id}`); }
  getLesson(id:string){ return this.http.get<LessonDetails>(`${this.baseUrl}/learning/lessons/${id}`); }
  getQuiz(id:string){ return this.http.get<Quiz>(`${this.baseUrl}/learning/lessons/${id}/quiz`); }
  submitQuiz(lessonId:string,answers:{questionId:string;answerId:string}[]){ return this.http.post<any>(`${this.baseUrl}/learning/lessons/${lessonId}/quiz/submit`,{answers}); }

  getFavorites(){ return this.http.get<any[]>(`${this.baseUrl}/learning/favorites`); }
  addFavorite(id:string){ return this.http.post<void>(`${this.baseUrl}/learning/lessons/${id}/favorite`,{}); }
  removeFavorite(id:string){ return this.http.delete<void>(`${this.baseUrl}/learning/lessons/${id}/favorite`); }
  getProfile(){ return this.http.get<ChildProfile>(`${this.baseUrl}/learning/profile`); }

  getChildren(){ return this.http.get<any[]>(`${this.baseUrl}/parents/children`); }
  linkChild(childEmail:string,linkCode:string){ return this.http.post<void>(`${this.baseUrl}/parents/children/link`,{childEmail,linkCode}); }
  getChildProgress(childId:string){ return this.http.get<any>(`${this.baseUrl}/parents/children/${childId}/progress`); }

  getDictionary(search='',category='',difficulty=''){
    let p=new HttpParams();
    if(search)p=p.set('q',search);
    if(category)p=p.set('category',category);
    if(difficulty)p=p.set('difficulty',difficulty);
    return this.http.get<SignEntry[]>(`${this.baseUrl}/dictionary`,{params:p});
  }

  getAchievements(){ return this.http.get<any[]>(`${this.baseUrl}/achievements`); }

  getAdminDashboard(){ return this.http.get<any>(`${this.baseUrl}/admin/dashboard`); }
  getUsers(search='',role=''){
    let p=new HttpParams();
    if(search)p=p.set('search',search);
    if(role)p=p.set('role',role);
    return this.http.get<any[]>(`${this.baseUrl}/admin/users`,{params:p});
  }
  setUserActive(id:string,active:boolean){ return this.http.patch<void>(`${this.baseUrl}/admin/users/${id}/active?active=${active}`,{}); }

  getContentCourses(search=''){ return this.http.get<any[]>(`${this.baseUrl}/content/courses`,{params:search?{search}:{}}); }
  getTeacherCourses(){ return this.getContentCourses(); }
  getContentCourse(id:string){ return this.http.get<any>(`${this.baseUrl}/content/courses/${id}`); }
  getCourseStructure(id:string){ return this.getContentCourse(id); }
  createCourse(payload:any){ return this.http.post<{id:string}>(`${this.baseUrl}/content/courses`,payload); }
  updateCourse(id:string,payload:any){ return this.http.put<void>(`${this.baseUrl}/content/courses/${id}`,payload); }
  deleteCourse(id:string){ return this.http.delete<void>(`${this.baseUrl}/content/courses/${id}`); }
  createLevel(courseId:string,payload:any){ return this.http.post<{id:string}>(`${this.baseUrl}/content/courses/${courseId}/levels`,payload); }
  addLevel(courseId:string,payload:any){ return this.createLevel(courseId,payload); }
  createLesson(levelId:string,payload:any){ return this.http.post<{id:string}>(`${this.baseUrl}/content/levels/${levelId}/lessons`,payload); }
  addLesson(levelId:string,payload:any){ return this.createLesson(levelId,payload); }
  updateLesson(id:string,payload:any){ return this.http.put<void>(`${this.baseUrl}/content/lessons/${id}`,payload); }
  deleteLesson(id:string){ return this.http.delete<void>(`${this.baseUrl}/content/lessons/${id}`); }
  createQuiz(lessonId:string,title:string){ return this.http.post<{id:string}>(`${this.baseUrl}/content/lessons/${lessonId}/quiz`,{title}); }
  createQuestion(quizId:string,payload:any){ return this.http.post<{id:string}>(`${this.baseUrl}/content/quizzes/${quizId}/questions`,payload); }
  publishCourse(id:string,published:boolean){ return this.http.patch<void>(`${this.baseUrl}/content/courses/${id}/publish?published=${published}`,{}); }
  publishLesson(id:string,published:boolean){ return this.http.patch<void>(`${this.baseUrl}/content/lessons/${id}/publish?published=${published}`,{}); }

  upload(file:File,type:'image'|'video'){
    const data=new FormData();
    data.append('file',file);
    return this.http.post<{url:string,fileName:string}>(`${this.baseUrl}/media/upload?type=${type}`,data);
  }

  absoluteMedia(url?:string|null){
    if(!url)return '';
    return url.startsWith('/media/')?this.serverUrl+url:url;
  }
}
