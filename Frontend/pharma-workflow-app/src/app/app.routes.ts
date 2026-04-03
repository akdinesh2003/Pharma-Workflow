import { Routes } from '@angular/router';
import { authGuard, roleGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { 
    path: 'login', 
    loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent)
  },
  { 
    path: 'dashboard', 
    loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'transactions', 
    loadComponent: () => import('./components/transactions-list/transactions-list.component').then(m => m.TransactionsListComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'transactions/create', 
    loadComponent: () => import('./components/create-transaction/create-transaction.component').then(m => m.CreateTransactionComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'transactions/:id/history', 
    loadComponent: () => import('./components/transaction-history/transaction-history.component').then(m => m.TransactionHistoryComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'transactions/:id/approve', 
    loadComponent: () => import('./components/approve-transaction/approve-transaction.component').then(m => m.ApproveTransactionComponent),
    canActivate: [roleGuard(['Manager', 'Admin'])]
  },
  { 
    path: 'workflow', 
    loadComponent: () => import('./components/workflow-details/workflow-details.component').then(m => m.WorkflowDetailsComponent),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '/dashboard' }
];
