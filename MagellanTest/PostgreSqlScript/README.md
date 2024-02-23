
-- Create database called Part
CREATE DATABASE Part;

-- Create the item table
CREATE TABLE IF NOT EXISTS item (
    id SERIAL PRIMARY KEY,
    item_name VARCHAR(50) NOT NULL,
    parent_item INTEGER,
    cost INTEGER NOT NULL,
    req_date DATE NOT NULL,
    CONSTRAINT fk_parent_item FOREIGN KEY (parent_item) REFERENCES item(id)
);

-- Insert data into the "item" table
INSERT INTO item (item_name, parent_item, cost, req_date) VALUES
('Item1', null, 500, '2024-02-20'),
('Sub1', 1, 200, '2024-02-10'),
('Sub2', 1, 300, '2024-01-05'),
('Sub3', 2, 300, '2024-01-02'),
('Sub4', 2, 400, '2024-01-02'),
('Item2', null, 600, '2024-03-15'),
('Sub1', 6, 200, '2024-02-25');

-- -- fetch 
SELECT * FROM item;


CREATE OR REPLACE FUNCTION Get_Total_Cost(p_item_name VARCHAR) RETURNS INTEGER AS $$
DECLARE
    total_cost INTEGER;
BEGIN
    -- Check if the specified item has a parent
    IF EXISTS (
        SELECT 1
        FROM item
        WHERE item_name = p_item_name AND parent_item IS NOT NULL
    ) THEN
        -- If the specified item has a parent, return NULL
        RETURN NULL;
    ELSE
        -- Calculate the total cost using a recursive common table expression (CTE)
        WITH RECURSIVE item_hierarchy AS (
            SELECT id, cost
            FROM item
            WHERE item_name = p_item_name
            UNION ALL
            SELECT i.id, i.cost
            FROM item i
            JOIN item_hierarchy ih ON i.parent_item = ih.id
        )
        SELECT INTO total_cost SUM(cost)
        FROM item_hierarchy;

        RETURN total_cost;
    END IF;
END;
$$ LANGUAGE plpgsql;


SELECT Get_Total_Cost('Sub2'); -- Returns NULL
SELECT Get_Total_Cost('Item2'); -- Returns 1700



