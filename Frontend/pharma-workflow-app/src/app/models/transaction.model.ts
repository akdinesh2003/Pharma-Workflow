export interface Transaction {
  id: number;
  drugName: string;
  batchNo: string;
  requestedBy: string;
  status: TransactionStatus;
  createdDate: string;
  updatedDate: string;
  filePath?: string;
  comments?: string;
  rowVersion?: number[];
}

export type TransactionStatus = 
  | 'Initiated' 
  | 'Approved' 
  | 'Rejected' 
  | 'Active' 
  | 'Inactive' 
  | 'Modified';

export interface CreateTransactionRequest {
  drugName: string;
  batchNo: string;
  requestedBy: string;
  comments?: string;
}

export interface UpdateTransactionRequest {
  drugName: string;
  batchNo: string;
  comments?: string;
  rowVersion?: number[];
}

export interface ApproveRejectRequest {
  actionBy: string;
  comments?: string;
  rowVersion?: number[];
}

export interface HistoryTransaction {
  historyId: number;
  transactionId: number;
  action: string;
  actionBy: string;
  comments?: string;
  actionDate: string;
  previousStatus?: string;
  newStatus?: string;
}

export interface DashboardStats {
  totalRequests: number;
  approved: number;
  pending: number;
  rejected: number;
  active: number;
  initiated: number;
}
