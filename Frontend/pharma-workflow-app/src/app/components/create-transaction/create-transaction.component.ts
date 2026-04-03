import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TransactionService } from '../../services/transaction.service';
import { AuthService } from '../../services/auth.service';
import { CreateTransactionRequest } from '../../models/transaction.model';

@Component({
  selector: 'app-create-transaction',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './create-transaction.component.html',
  styleUrls: ['./create-transaction.component.css']
})
export class CreateTransactionComponent {
  request: CreateTransactionRequest = {
    drugName: '',
    batchNo: '',
    requestedBy: '',
    comments: ''
  };
  
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  currentUser$ = this.authService.currentUser$;

  constructor(
    private transactionService: TransactionService,
    private authService: AuthService,
    private router: Router
  ) {
    const user = this.authService.getCurrentUser();
    if (user) {
      this.request.requestedBy = user.fullName;
    }
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isLoading = true;

    this.transactionService.createTransaction(this.request).subscribe({
      next: (response) => {
        this.successMessage = 'Request created successfully!';
        this.isLoading = false;
        setTimeout(() => {
          this.router.navigate(['/transactions']);
        }, 1500);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to create request';
        this.isLoading = false;
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
