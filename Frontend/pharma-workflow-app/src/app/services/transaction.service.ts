import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  Transaction, 
  CreateTransactionRequest, 
  UpdateTransactionRequest,
  ApproveRejectRequest,
  HistoryTransaction,
  DashboardStats
} from '../models/transaction.model';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private apiUrl = 'https://localhost:7001/api/transactions';

  constructor(private http: HttpClient) {}

  createTransaction(request: CreateTransactionRequest): Observable<Transaction> {
    return this.http.post<Transaction>(this.apiUrl, request);
  }

  getAllTransactions(status?: string, search?: string): Observable<Transaction[]> {
    let params = new HttpParams();
    if (status) params = params.set('status', status);
    if (search) params = params.set('search', search);
    
    return this.http.get<Transaction[]>(this.apiUrl, { params });
  }

  getTransactionById(id: number): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.apiUrl}/${id}`);
  }

  approveTransaction(id: number, request: ApproveRejectRequest): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.apiUrl}/${id}/approve`, request);
  }

  activateTransaction(id: number, comments?: string): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.apiUrl}/${id}/activate`, { comments });
  }

  completeTransaction(id: number, comments?: string): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.apiUrl}/${id}/complete`, { comments });
  }

  rejectTransaction(id: number, request: ApproveRejectRequest): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.apiUrl}/${id}/reject`, request);
  }

  modifyTransaction(id: number, request: UpdateTransactionRequest): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.apiUrl}/${id}/modify`, request);
  }

  deleteTransaction(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getTransactionHistory(id: number): Observable<HistoryTransaction[]> {
    return this.http.get<HistoryTransaction[]>(`${this.apiUrl}/${id}/history`);
  }

  getDashboardStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.apiUrl}/dashboard/stats`);
  }
}
