﻿using System;
using System.Collections.Generic;
using Entities.BoardEntities;
using Repository;
using System.Data.Entity;
using System.Linq;

namespace DataAccessLayer
{
    public class BoardDao : IBoardDao
    {

        private IQueryable<BoardEntity> IncludeDays(IQueryable<BoardEntity> query)
        {
            return query.Include(board => board.Days);
        }


        private IQueryable<BoardEntity> IncludeCategories(IQueryable<BoardEntity> query)
        {
            return query.Include(board => board.Days.Select(day => day.Categories));
        }

        private IQueryable<BoardEntity> IncludeItems(IQueryable<BoardEntity> query)
        {
            return query.Include(board => board.Days.Select(day => day.Categories.Select(category => category.Items)));
        }

        private IQueryable<BoardEntity> IncludeAllBoardProperties(IQueryable<BoardEntity> query)
        {
            // TODO: refactor this with extensiosn methods
            return IncludeItems(IncludeCategories(IncludeDays(query)));
        }


        public BoardEntity GetBoardById(long boardId)
        {
            using (var repository = new FoodPlanAppContext())
            {
                var boardEntity = IncludeAllBoardProperties(repository.BoardEntities).
                                FirstOrDefault(board => board.Id == boardId);

                return boardEntity;

            }
        }

        public void UpdateBoard(BoardEntity updatedBoard)
        {
            using (var repository = new FoodPlanAppContext())
            {
                UpdateAllBoardProperties(updatedBoard, repository);

                repository.SaveChanges();
            }
        }

        private static void UpdateAllBoardProperties(BoardEntity updatedBoard, FoodPlanAppContext repository)
        {
            //TODO: Refactor this method in a nicer way

            repository.BoardEntities.Attach(updatedBoard);
            repository.Entry(updatedBoard).State = EntityState.Modified;


            foreach (var day in updatedBoard.Days)
            {
                repository.DayEntities.Attach(day);
                repository.Entry(day).State = EntityState.Modified;

                foreach (var category in day.Categories)
                {
                    repository.CategoryEntities.Attach(category);
                    repository.Entry(category).State = EntityState.Modified;

                    foreach (var item in category.Items)
                    {
                        repository.ItemEntities.Attach(item);
                        repository.Entry(item).State = EntityState.Modified;
                    }
                }
            }
        }
    }
}
