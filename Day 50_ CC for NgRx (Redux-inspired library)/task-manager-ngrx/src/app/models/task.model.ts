export interface Task {
  id: string;
  title: string;
  description: string;
  completed: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface TaskState {
  tasks: Task[];
  loading: boolean;
  error: string | null;
}