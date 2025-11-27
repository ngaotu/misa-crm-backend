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
  -- Create date: 16/11/2025
  -- Description: Lấy danh sách khách hàng có hỗ trợ phân trang, sắp xếp và tìm kiếm chung theo Tên, Email, SĐT.
  -- Modified by:
  -- Code chay thu: 
  -- CALL proc_customers_paging_and_sort('Nguyễn', 1, 10, 'CustomerFullName', 'DESC');
  -- ;
  DECLARE v_offset int DEFAULT 0;
  DECLARE v_sort_column varchar(50) DEFAULT 'customer_code';
  DECLARE v_sort_dir varchar(4) DEFAULT 'ASC';
  DECLARE v_where_clause varchar(1000) DEFAULT ' WHERE is_deleted = 0 ';
  DECLARE v_search_pattern varchar(257) DEFAULT NULL;

  -- Tính offset cho phân trang
  SET v_offset = (p_page_number - 1) * p_page_size;

  -- Xử lý sort column
  SET v_sort_column = CASE WHEN p_sort_by = 'CustomerCode' THEN 'customer_code' WHEN p_sort_by = 'CustomerFullName' THEN 'customer_full_name' WHEN p_sort_by = 'CustomerEmail' THEN 'customer_email' WHEN p_sort_by = 'CustomerPhone' THEN 'customer_phone' WHEN p_sort_by = 'CustomerType' THEN 'customer_type' WHEN p_sort_by = 'CustomerTaxCode' THEN 'customer_tax_code' WHEN p_sort_by = 'CustomerShippingAddr' THEN 'customer_shipping_addr' WHEN p_sort_by = 'CustomerLastPurchaseDate' THEN 'customer_last_purchase_date' WHEN p_sort_by = 'CustomerLastestPurchasedItems' THEN 'customer_lastest_purchased_items' WHEN p_sort_by = 'CustomerPurchasedItems' THEN 'customer_purchased_items' ELSE 'customer_code' END;

  -- Xử lý sort direction
  SET v_sort_dir = CASE WHEN UPPER(p_sort_direction) = 'DESC' THEN 'DESC' ELSE 'ASC' END;

  -- Thêm điều kiện tìm kiếm chung
  IF p_query IS NOT NULL
    AND p_query != '' THEN
    -- Bọc chuỗi tìm kiếm bằng ký tự đại diện %
    SET v_search_pattern = CONCAT('%', p_query, '%');

    -- Nối điều kiện tìm kiếm vào chuỗi WHERE cơ sở. 
    SET v_where_clause = CONCAT(v_where_clause, ' 
            AND (
                customer_full_name LIKE ? OR
                customer_email LIKE ? OR
                customer_phone LIKE ?
            )
        ');
    -- Gán giá trị tham số
    SET @p1 = v_search_pattern;
    SET @p2 = v_search_pattern;
    SET @p3 = v_search_pattern;
  END IF;

  -- TRUY VẤN LẤY DỮ LIỆU
  SET @sql = CONCAT('SELECT * FROM customer ', v_where_clause);
  -- Thêm sắp xếp và phân trang
  SET @sql = CONCAT(@sql, ' ORDER BY ', v_sort_column, ' ', v_sort_dir);
  SET @sql = CONCAT(@sql, ' LIMIT ', p_page_size, ' OFFSET ', v_offset);

  -- Thực thi query lấy dữ liệu
  PREPARE stmt FROM @sql;
  IF v_search_pattern IS NOT NULL THEN
    EXECUTE stmt USING @p1, @p2, @p3;
  ELSE
    EXECUTE stmt;
  END IF;
  DEALLOCATE PREPARE stmt;

  -- Lấy tổng số bản ghi
  SET @count_sql = CONCAT('SELECT COUNT(*) AS TotalRecords FROM customer ', v_where_clause);

  PREPARE count_stmt FROM @count_sql;

  -- Thực thi an toàn: kiểm tra xem có tham số tìm kiếm không
  IF v_search_pattern IS NOT NULL THEN
    EXECUTE count_stmt USING @p1, @p2, @p3;
  ELSE
    EXECUTE count_stmt;
  END IF;
  DEALLOCATE PREPARE count_stmt;

END
$$

DELIMITER ;