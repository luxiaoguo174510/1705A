using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CookieTest.Controllers
{
    public class GoodsController : Controller
    {

        //商品数据列表集合（用于显示，可以从数据查询获取）
        List<Goods> goodsList = new List<Goods>();
        #region 获取数据
        public GoodsController()
        {
            for (int i = 1; i <= 30; i++)
            {
                goodsList.Add(new Goods
                {
                    ID = i,
                    GoodsNo = $"NO{i}",
                    GoodsTitle = $"Title{i}",
                    GoodsSubTitle = $"SubTitle{i}",
                    GoodsBland = $"Bland{i}",
                    GoodsType = $"Type{i}",
                    MarketPrice = i,
                    CostPrice = i * 120 / 100,
                    GoodsDescription = $"Desciprtion{i}",
                    GoodsCount = 100,
                    GoodsOneImage = "/scripts/1903548109.jpg"
                });
            }
        }
        #endregion

        /// <summary>
        /// 商品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            #region 前台显示cookie的数量

            HttpCookie cookie = Request.Cookies["testcookie"];
            if (cookie != null)
            {
                string _cookieList = HttpUtility.UrlDecode(cookie.Value, Encoding.GetEncoding("UTF-8"));
                List<Goods> ids = JsonConvert.DeserializeObject<List<Goods>>(_cookieList); 
                ViewBag.CarNumber = ids != null ? ids.Count : 0;
            }
            else
            {
                ViewBag.CarNumber = 0;
            }

            #endregion

            return View(goodsList);
        }

        /// <summary>
        /// 添加到购物车（Cookie）
        /// </summary>
        /// <param name="id"></param>
        public int PutCar(string id)
        {
            List<Goods> cookieGoodsList = new List<Goods>();
            Goods _goods = goodsList.Where(x=>x.ID.ToString() == id).First();// goodsList.Find(x => x.ID.ToString() == id); //查找该id商品的所有信息（可以直接当参数传进来或从数据库获取）
            _goods.CarCount = 1;//购物车的数量默认为1
            /*
             * Cookie操作步骤
             * 1、获取自定义（之前）设置的cookie对象
             * 2、判断该cookie对象是否存在
             * 3、cookie对象不存在，
             *          则创建新cookie
             * 4、cookie对象存在,
             *          则判断该cookie是否有值
             *          
             *          4.1、没有值
             *                  则添加（注：cookie里只能放字符串，并有大小限制，这里只作演示）
             *                  
             *          4.2、cookie里有值
             *                  4.2.1、先获取里面的值
             *                  4.2.2、判断要添加的商品，之前有没有存在过（从cookie里取出来的值）
             *                          不存在，则添加。存在，则修改数量。
             * 5、将数据添加到cookie
             * 6、设置cookie过期时间
             * 7、将cookie到response中
             * **/

            //第1步
            HttpCookie cookie = Request.Cookies["testcookie"];

            //第2步
            if (cookie == null)
            {
                //第3步
                cookie = new HttpCookie("testcookie");
            }

            
            //第4步
            if (string.IsNullOrEmpty(cookie.Value))
            {
                //第4.1步（添加）
                cookieGoodsList.Add(_goods);
            }
            else
            {
                //将文本信息进行编码，主要是为了识别商品中的中文信息
                string _cookieList = HttpUtility.UrlDecode(cookie.Value, Encoding.GetEncoding("UTF-8"));
                //我们的cookie存放的的Goods列表对象，所以将cookie的值取出来后，转换为List<Goods>
                cookieGoodsList = JsonConvert.DeserializeObject<List<Goods>>(_cookieList);

                //第4.2.2步
                int _index = cookieGoodsList.FindLastIndex(delegate(Goods g){ return g.ID == _goods.ID; });
                if (_index < 0)
                {
                    cookieGoodsList.Add(_goods);
                }
                else {
                    cookieGoodsList[_index].CarCount += 1;
                }
            }

            //第5步
            var value = JsonConvert.SerializeObject(cookieGoodsList);
            cookie.Value = value;

            //第6步
            cookie.Expires = DateTime.Now.AddMinutes(60);
            //第7步
            Response.AppendCookie(cookie);

            return 1;//cookieGoodsList.Count;//为了测试结果,返回购物车中记录条数
        }


        public ActionResult CarList()
        {
            List<Goods> goodsList = new List<Goods>();

            HttpCookie cookie = Request.Cookies["testcookie"];
            if (cookie != null)
            {
                string _cookieList = HttpUtility.UrlDecode(cookie.Value, Encoding.GetEncoding("UTF-8"));
                goodsList = JsonConvert.DeserializeObject<List<Goods>>(_cookieList); 
            }
            return View(goodsList);
        }

        /// <summary>
        /// 结算操作（清空购物车）
        /// </summary>
        /// <returns></returns>
        public int ClearCar()
        {
            //结算操作
            //........

            //结算完成，设置cookie过期
            Response.Cookies["testcookie"].Expires = DateTime.Now.AddDays(-1);

            return 1;
        }
    }


    #region 商品类
    public class Goods
    {

        public int ID { get; set; }
        public string GoodsNo { get; set; }
        public string GoodsTitle { get; set; }
        public string GoodsSubTitle { get; set; }
        public string GoodsBland { get; set; }
        public string GoodsType { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string GoodsDescription { get; set; }
        public string GoodsContent { get; set; }
        public int GoodsCount { get; set; }
        public string GoodsState { get; set; }
        public string Sort { get; set; }


        /// <summary>
        /// 显示用的一条图片
        /// </summary>
        public string GoodsOneImage { get; set; }

        public int CarCount { get; set; } //购物车里放的该对象数量


    }
    #endregion
}