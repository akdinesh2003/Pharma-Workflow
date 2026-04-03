import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TransactionService } from '../../services/transaction.service';
import { AuthService } from '../../services/auth.service';
import { Transaction, ApproveRejectRequest } from '../../models/transaction.model';

@Component({
  selector: 'app-approve-transaction',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './approve-transaction.component.html',
  styleUrls: ['./approve-transaction.component.css']
})
export class ApproveTransactionComponent implements OnInit {
  transaction: Transaction | null = null;
  comments = '';
  isLoading = true;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  currentUser$ = this.authService.currentUser$;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private transactionService: TransactionService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadTransaction(id);
  }

  loadTransaction(id: number): void {
    this.transactionService.getTransactionById(id).subscribe({
      next: (data) => {
        this.transaction = data;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load transaction';
        this.isLoading = false;
      }
    });
  }

  approve(): void {
    if (!this.transaction) return;

    const user = this.authService.getCurrentUser();
    const request: ApproveRejectRequest = {
      actionBy: user?.fullName || 'Unknown',
      comments: this.comments,
      rowVersion: this.transaction.rowVersion
    };

    this.isSubmitting = true;
    this.transactionService.approveTransaction(this.transaction.id, request).subscribe({
      next: () => {
        this.successMessage = 'Transaction approved successfully!';
        setTimeout(() => this.router.navigate(['/transactions']), 1500);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to approve transaction';
        this.isSubmitting = false;
      }
    });
  }

  reject(): void {
    if (!this.transaction) return;

    const user = this.authService.getCurrentUser();
    const request: ApproveRejectRequest = {
      actionBy: user?.fullName || 'Unknown',
      comments: this.comments,
      rowVersion: this.transaction.rowVersion
    };

    this.isSubmitting = true;
    this.transactionService.rejectTransaction(this.transaction.id, request).subscribe({
      next: () => {
        this.successMessage = 'Transaction rejected successfully!';
        setTimeout(() => this.router.navigate(['/transactions']), 1500);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to reject transaction';
        this.isSubmitting = false;
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
