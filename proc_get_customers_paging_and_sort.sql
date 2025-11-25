USE misa_crm_2025;

DROP PROCEDURE IF EXISTS proc_customers_paging_and_sort;

DELIMITER $$

CREATE 
	DEFINER = 'root'@'localhost'
PROCEDURE proc_customers_paging_and_sort(IN p_query varchar(255),
IN p_page_number int,
IN p_page_size int,
IN p_sort_by varchar(50),
IN p_sort_direction varchar(4))
BEGIN
  -- ================================
  -- Author: tungnguyen
  -- Create date: 11/7/2025
  -- Description: <Mo ta muc dich, chuc nang>
  -- Modified by:
  -- Code chay thu: 
  -- CALL 
  -- ;
  DECLARE v_offset int DEFAULT 0;
  DECLARE v_sort_column varchar(50) DEFAULT 'customer_code';
  DECLARE v_sort_dir varchar(4) DEFAULT 'ASC';
  -- Tính offset cho phân trang
  SET v_offset = (p_page_number - 1) * p_page_size;

  -- Xử lý sort column
  SET v_sort_column = CASE WHEN p_sort_by = 'CustomerCode' THEN 'customer_code' WHEN p_sort_by = 'CustomerFullName' THEN 'customer_full_name' WHEN p_sort_by = 'CustomerEmail' THEN 'customer_email' WHEN p_sort_by = 'CustomerPhone' THEN 'customer_phone' WHEN p_sort_by = 'CustomerType' THEN 'customer_type' WHEN p_sort_by = 'CustomerTaxCode' THEN 'customer_tax_code' WHEN p_sort_by = 'CustomerShippingAddr' THEN 'customer_shipping_addr' WHEN p_sort_by = 'CustomerLastPurchaseDate' THEN 'customer_last_purchase_date' WHEN p_sort_by = 'CustomerLastestPurchasedItems' THEN 'customer_lastest_purchased_items' WHEN p_sort_by = 'CustomerPurchasedItems' THEN 'customer_purchased_items' ELSE 'customer_code' END;

  -- Xử lý sort direction
  SET v_sort_dir = CASE WHEN UPPER(p_sort_direction) = 'DESC' THEN 'DESC' ELSE 'ASC' END;

  -- Danh sách customer
  SET @sql = 'SELECT * FROM customer WHERE is_deleted = 0';

  -- Thêm điều kiện tìm kiếm chung
  IF p_query IS NOT NULL
    AND p_query != '' THEN
    SET @sql = CONCAT(@sql, ' AND (
           customer_full_name LIKE ''%', p_query, '%'' OR
           customer_email LIKE ''%', p_query, '%'' OR  
           customer_phone LIKE ''%', p_query, '%''
       )');
  END IF;
  -- Thêm sắp xếp và phân trang
  SET @sql = CONCAT(@sql, ' ORDER BY ', v_sort_column, ' ', v_sort_dir);
  SET @sql = CONCAT(@sql, ' LIMIT ', p_page_size, ' OFFSET ', v_offset);

  -- Thực thi query lấy dữ liệu
  PREPARE stmt FROM @sql;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;

  -- Lấy tổng số bản ghi
  SET @count_sql = 'SELECT COUNT(*) AS TotalRecords FROM customer WHERE is_deleted = 0';

  -- Thêm lại điều kiện tìm kiếm cho count
  IF p_query IS NOT NULL
    AND p_query != '' THEN
    SET @count_sql = CONCAT(@count_sql, ' AND (
             customer_full_name LIKE ''%', p_query, '%'' OR
             customer_email LIKE ''%', p_query, '%'' OR  
             customer_phone LIKE ''%', p_query, '%''
         )');
  END IF;

  -- Thực thi query đếm
  PREPARE count_stmt FROM @count_sql;
  EXECUTE count_stmt;
  DEALLOCATE PREPARE count_stmt;

END
$$

DELIMITER ;