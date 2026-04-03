import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { TransactionService } from '../../services/transaction.service';
import { AuthService } from '../../services/auth.service';
import { Transaction, HistoryTransaction } from '../../models/transaction.model';

@Component({
  selector: 'app-transaction-history',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './transaction-history.component.html',
  styleUrls: ['./transaction-history.component.css']
})
export class TransactionHistoryComponent implements OnInit {
  transaction: Transaction | null = null;
  history: HistoryTransaction[] = [];
  isLoading = true;
  errorMessage = '';
  currentUser$ = this.authService.currentUser$;

  constructor(
    private route: ActivatedRoute,
    private transactionService: TransactionService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadTransactionAndHistory(id);
  }

  loadTransactionAndHistory(id: number): void {
    this.transactionService.getTransactionById(id).subscribe({
      next: (data) => {
        this.transaction = data;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load transaction';
      }
    });

    this.transactionService.getTransactionHistory(id).subscribe({
      next: (data) => {
        this.history = data;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load history';
        this.isLoading = false;
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
