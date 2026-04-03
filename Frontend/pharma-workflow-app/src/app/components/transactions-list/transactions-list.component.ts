import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { TransactionService } from '../../services/transaction.service';
import { AuthService } from '../../services/auth.service';
import { Transaction } from '../../models/transaction.model';

@Component({
  selector: 'app-transactions-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './transactions-list.component.html',
  styleUrls: ['./transactions-list.component.css']
})
export class TransactionsListComponent implements OnInit {
  transactions: Transaction[] = [];
  filteredTransactions: Transaction[] = [];
  isLoading = true;
  searchTerm = '';
  statusFilter = '';
  currentUser$ = this.authService.currentUser$;

  constructor(
    private transactionService: TransactionService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // Check for query parameters
    this.route.queryParams.subscribe(params => {
      if (params['status']) {
        this.statusFilter = params['status'];
      }
      this.loadTransactions();
    });
  }

  loadTransactions(): void {
    this.isLoading = true;
    this.transactionService.getAllTransactions().subscribe({
      next: (data) => {
        this.transactions = data;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading transactions:', error);
        this.isLoading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredTransactions = this.transactions.filter(t => {
      const matchesSearch = !this.searchTerm || 
        t.drugName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        t.batchNo.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      const matchesStatus = !this.statusFilter || t.status === this.statusFilter;
      
      return matchesSearch && matchesStatus;
    });
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onStatusFilterChange(): void {
    this.applyFilters();
  }

  viewHistory(id: number): void {
    this.router.navigate(['/transactions', id, 'history']);
  }

  approveReject(id: number): void {
    this.router.navigate(['/transactions', id, 'approve']);
  }

  canApproveReject(): boolean {
    return this.authService.hasAnyRole(['Manager', 'Admin']);
  }

  canDelete(): boolean {
    return this.authService.hasRole('Admin');
  }

  deleteTransaction(id: number, drugName: string): void {
    if (confirm(`Are you sure you want to delete ${drugName}? This will mark it as deleted.`)) {
      this.transactionService.deleteTransaction(id).subscribe({
        next: () => {
          alert('Transaction deleted successfully!');
          this.loadTransactions();
        },
        error: (error) => {
          console.error('Error deleting transaction:', error);
          alert('Failed to delete transaction. Please try again.');
        }
      });
    }
  }

  activateTransaction(id: number, drugName: string): void {
    const comments = prompt(`Activate ${drugName} for processing?\n\nPlease enter comments (optional):`);
    
    // If user clicks Cancel, comments will be null
    if (comments === null) {
      return; // User cancelled
    }
    
    // User clicked OK (comments can be empty string or have text)
    this.transactionService.activateTransaction(id, comments || undefined).subscribe({
      next: () => {
        alert('Transaction activated successfully!');
        this.loadTransactions();
      },
      error: (error) => {
        console.error('Error activating transaction:', error);
        alert(error.error?.message || 'Failed to activate transaction. Please try again.');
      }
    });
  }

  completeTransaction(id: number, drugName: string): void {
    const comments = prompt(`Mark ${drugName} as completed?\n\nPlease enter comments (optional):`);
    
    // If user clicks Cancel, comments will be null
    if (comments === null) {
      return; // User cancelled
    }
    
    // User clicked OK (comments can be empty string or have text)
    this.transactionService.completeTransaction(id, comments || undefined).subscribe({
      next: () => {
        alert('Transaction completed successfully!');
        this.loadTransactions();
      },
      error: (error) => {
        console.error('Error completing transaction:', error);
        alert(error.error?.message || 'Failed to complete transaction. Please try again.');
      }
    });
  }

  modifyTransaction(id: number, currentDrugName: string, currentBatchNo: string): void {
    const newDrugName = prompt(`Modify Drug Name:\n\nCurrent: ${currentDrugName}\nEnter new Drug Name:`, currentDrugName);
    
    if (newDrugName === null) {
      return; // User cancelled
    }
    
    const newBatchNo = prompt(`Modify Batch Number:\n\nCurrent: ${currentBatchNo}\nEnter new Batch Number:`, currentBatchNo);
    
    if (newBatchNo === null) {
      return; // User cancelled
    }
    
    const comments = prompt(`Enter modification comments (optional):`);
    
    if (comments === null) {
      return; // User cancelled
    }
    
    const request = {
      drugName: newDrugName.trim(),
      batchNo: newBatchNo.trim(),
      comments: comments.trim() || 'Transaction modified'
    };
    
    this.transactionService.modifyTransaction(id, request).subscribe({
      next: () => {
        alert('Transaction modified successfully! Status changed to Modified.');
        this.loadTransactions();
      },
      error: (error) => {
        console.error('Error modifying transaction:', error);
        alert(error.error?.message || 'Failed to modify transaction. Please try again.');
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
