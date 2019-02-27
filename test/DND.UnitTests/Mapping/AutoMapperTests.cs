using AspNetCore.Base.Mapping;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using DND.ApplicationServices.CMS.CarouselItems.Dtos;
using DND.Domain.CMS.CarouselItems;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DND.UnitTests.Mapping
{
    public class AutoMapperTests
    {
        [Fact]
        public async Task DtoToEntity()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddExpressionMapping();
                new CarouselItemDto().CreateMappings(cfg);
            });

            var expressionBuider = new AutoMapper.QueryableExtensions.ExpressionBuilder(config);

            var mapper = config.CreateMapper();

            //dto > entity
            var dto = new CarouselItemDto();
            var entity = mapper.Map<CarouselItem>(dto);
            entity.CreatedOn = new DateTime(2019, 1, 1);

            //entity > dto
            var dto2 = mapper.Map<CarouselItemDto>(entity);
            Assert.Equal(new DateTime(2019, 1, 1), dto2.CreatedOn);

            //Mapped OrderBy
            var orderBy = AutoMapperHelper.GetOrderBy<CarouselItemDto>("CreatedOn", "desc");
            var mappedOrderBy = AutoMapperHelper.GetMappedOrderBy<CarouselItemDto, CarouselItem>(mapper, orderBy);
            mappedOrderBy.Compile();

            //Mapped Include
            Expression<Func<CarouselItemDto, object>> include = d => d.Title;
            var mappedIncludes = AutoMapperHelper.GetMappedIncludes<CarouselItemDto, CarouselItem>(mapper, include);
            mappedIncludes.First().Compile();
        }
    }
}
