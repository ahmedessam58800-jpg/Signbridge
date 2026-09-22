export interface AuthResponse {
  accessToken:string;
  refreshToken:string;
  accessTokenExpiresAtUtc:string;
  userId:string;
  fullName:string;
  email:string;
  role:'Admin'|'Teacher'|'Parent'|'Child';
  linkCode?:string|null;
}

export interface CourseSummary {
  id:string;
  title:string;
  description:string;
  coverImageUrl?:string|null;
  totalLessons:number;
  completedLessons:number;
  progressPercent:number;
}

export interface LessonItem {
  id:string;
  title:string;
  thumbnailUrl?:string|null;
  durationMinutes:number;
  order:number;
  isUnlocked:boolean;
  isCompleted:boolean;
  bestScore:number;
}

export interface CourseDetails {
  id:string;
  title:string;
  description:string;
  coverImageUrl?:string|null;
  levels:{id:string;title:string;order:number;lessons:LessonItem[]}[];
}

export interface LessonDetails {
  id:string;
  title:string;
  summary:string;
  videoUrl?:string|null;
  thumbnailUrl?:string|null;
  durationMinutes:number;
  xpReward:number;
  minimumPassingScore:number;
  isFavorite:boolean;
  isCompleted:boolean;
  bestScore:number;
}

export interface Quiz {
  id:string;
  title:string;
  questions:{id:string;text:string;order:number;answers:{id:string;text:string}[]}[];
}

export interface SignEntry {
  id:string;
  word:string;
  arabicWord:string;
  category:string;
  difficulty:string;
  description:string;
  videoUrl?:string|null;
  imageUrl?:string|null;
}

export interface ChildProfile {
  id:string;
  fullName:string;
  totalXp:number;
  currentStreak:number;
  completedLessons:number;
  quizAttempts:number;
  averageQuizScore:number;
  levelName:string;
  achievements:{key:string;title:string;description:string;icon:string;unlocked:boolean}[];
}
