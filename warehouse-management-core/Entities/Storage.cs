﻿namespace warehouse_management_core.Entities;

public class Storage : IEntity
{
    public Id Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }

    public int Temperature { get; set; }

   
    public virtual ICollection<Item> Items { get; set; }
    public virtual ICollection<Shop> Shops { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
}
