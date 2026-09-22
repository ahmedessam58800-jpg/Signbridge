import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from './api.service';
import { AuthResponse } from '../models';
import { tap } from 'rxjs/operators';

@Injectable({ providedIn:'root' })
export class AuthService {
  private key='signbridge-auth';

  constructor(private api:ApiService,private router:Router){}

  login(email:string,password:string){
    return this.api.login(email,password).pipe(
      tap(auth=>localStorage.setItem(this.key,JSON.stringify(auth)))
    );
  }

  register(payload:{fullName:string;email:string;password:string;role:number;dateOfBirth:string|null}){
    return this.api.register(payload).pipe(
      tap(auth=>localStorage.setItem(this.key,JSON.stringify(auth)))
    );
  }

  get session():AuthResponse|null{
    const raw=localStorage.getItem(this.key);
    if(!raw)return null;
    try{return JSON.parse(raw) as AuthResponse}catch{return null}
  }

  get token(){return this.session?.accessToken??null}
  get role(){return this.session?.role??null}

  logout(){
    const refresh=this.session?.refreshToken;
    const finish=()=>{localStorage.removeItem(this.key);this.router.navigateByUrl('/login')};
    if(refresh)this.api.logout(refresh).subscribe({next:finish,error:finish});
    else finish();
  }

  goHome(){
    const role=this.role;
    if(role==='Child')return this.router.navigateByUrl('/child');
    if(role==='Parent')return this.router.navigateByUrl('/parent');
    if(role==='Teacher')return this.router.navigateByUrl('/teacher');
    if(role==='Admin')return this.router.navigateByUrl('/admin');
    return this.router.navigateByUrl('/login');
  }
}
