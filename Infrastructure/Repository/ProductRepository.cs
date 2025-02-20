﻿using Dapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
  public class ProductRepository : Repository<Product>, IProductRepository
  {
    private readonly ECommerceDbContext _dbContext;
    DbSet<Product> _dbSet;
    public ProductRepository(ECommerceDbContext dbContext, IConfiguration configuration) : base(dbContext, configuration)
    {
      _dbContext = dbContext;
      _dbSet = dbContext.Set<Product>();
    }

    public void CreateDapper(Product product)
    {
      try
      {
        var query = @"INSERT INTO ""Products"" (""Id"", ""Name"", ""Price"", ""Description"", ""CreatedOn"", ""IsActive"") VALUES (@Id, @Name, @Price, @Description, @CreatedOn, @IsActive)";

        var parameters = new DynamicParameters();
        parameters.Add("Id", product.Id);
        parameters.Add("Name", product.Name);
        parameters.Add("Price", product.Price);
        parameters.Add("Description", product.Description);
        parameters.Add("CreatedOn", product.CreatedOn);
        parameters.Add("IsActive", product.IsActive);

        using(var connection = CreateConnection())
        {
          if(connection.State != System.Data.ConnectionState.Open)
          {
            connection.Open();
          }

          connection.Execute(query, parameters);
        }

      }
      catch(Exception ex)
      {
        throw new Exception(ex.Message, ex);
      }
    }

    public Product GetById(Guid productId)
    {
      return _dbSet.SingleOrDefault(product => product.Id == productId);
    }

    public List<Product> GetAllDapper()
    {
      try
      {
        var query = @"SELECT * FROM ""Products"" ";

        using(var connection = CreateConnection())
        {
          if(connection.State != System.Data.ConnectionState.Open)
          {
            connection.Open();
          }

          return connection.Query<Product>(query).ToList();
        }

      }
      catch(Exception ex)
      {
        throw new Exception(ex.Message, ex);
      }
    }
  }


}
