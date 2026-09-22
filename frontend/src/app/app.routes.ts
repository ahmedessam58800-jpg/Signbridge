import { Routes } from '@angular/router';
import { roleGuard } from './core/auth.guard';
import { LoginComponent } from './pages/login.component';
import { RegisterComponent } from './pages/register.component';
import { ChildDashboardComponent } from './pages/child-dashboard.component';
import { CourseComponent } from './pages/course.component';
import { LessonComponent } from './pages/lesson.component';
import { QuizComponent } from './pages/quiz.component';
import { DictionaryComponent } from './pages/dictionary.component';
import { FavoritesComponent } from './pages/favorites.component';
import { ProfileComponent } from './pages/profile.component';
import { ParentDashboardComponent } from './pages/parent-dashboard.component';
import { TeacherDashboardComponent } from './pages/teacher-dashboard.component';
import { AdminDashboardComponent } from './pages/admin-dashboard.component';
import { AchievementsComponent } from './pages/achievements.component';

export const routes:Routes=[
  {path:'',pathMatch:'full',redirectTo:'login'},
  {path:'login',component:LoginComponent},
  {path:'register',component:RegisterComponent},

  {path:'child',component:ChildDashboardComponent,canActivate:[roleGuard('Child')]},
  {path:'child/course/:id',component:CourseComponent,canActivate:[roleGuard('Child')]},
  {path:'child/lesson/:id',component:LessonComponent,canActivate:[roleGuard('Child')]},
  {path:'child/lesson/:id/quiz',component:QuizComponent,canActivate:[roleGuard('Child')]},
  {path:'child/dictionary',component:DictionaryComponent,canActivate:[roleGuard('Child')]},
  {path:'child/favorites',component:FavoritesComponent,canActivate:[roleGuard('Child')]},
  {path:'child/profile',component:ProfileComponent,canActivate:[roleGuard('Child')]},
  {path:'child/achievements',component:AchievementsComponent,canActivate:[roleGuard('Child')]},

  {path:'dictionary',component:DictionaryComponent,canActivate:[roleGuard('Parent','Teacher','Admin')]},
  {path:'parent',component:ParentDashboardComponent,canActivate:[roleGuard('Parent')]},
  {path:'teacher',component:TeacherDashboardComponent,canActivate:[roleGuard('Teacher','Admin')]},
  {path:'admin',component:AdminDashboardComponent,canActivate:[roleGuard('Admin')]},

  {path:'**',redirectTo:'login'}
];
