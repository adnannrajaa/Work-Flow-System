using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkFlowProject.Models.DataBaseModel;
using WorkFlowProject.Models.Forum;

namespace WorkFlowProject.ViewModels.Forum
{
    public class PostViewModel
    {
        public IList<PostModel> postModel { get; set; }
        public IList<CommentModel> commentModel { get; set; }

        public IList<LikeModel> likeModel { get; set; }
        public IList<DisLikeModel> dislikeModel { get; set; }


        public int? totalLikes { get; set; }

        public static PostViewModel PostCommentLikeData(WorkFlowDbContext db)
        {
            var PostResult = db.Posts.Join(db.Users, p => p.UserId, u => u.UserId, (x, y) => new
            {
                pos = x,
                use = y
            }).Join(db.UsersDetails, us => us.use.UserId, ud => ud.UserId, (us, ud) => new
            {
                us.pos,
                us.use,
                ud


            }).Select(s => new PostModel
            {
                PostId = s.pos.PostId,
                Account = s.use.Account,
                FullName = s.ud.FirstName + " " + s.ud.LastName,
                ImagePath = s.ud.ImagePath,
                CreatedTime = s.pos.CreatedTime,
                PostContent = s.pos.PostContent,
                PostLikes = s.use.PostLikes.Count(),

            }).OrderByDescending(d=>d.PostId).ToList();

            var CommentResult = db.PostComments.Join(db.Users, pc => pc.UserId, u => u.UserId, (x, y) => new
            {
                pcom = x,
                use = y
            }).Join(db.UsersDetails, us => us.use.UserId, ud => ud.UserId, (us, ud) => new
            {
                us.pcom,
                us.use,
                ud


            }).Where(s=>s.pcom.Status == true).Select(f => new CommentModel
            {
                CommentId = f.pcom.CommentId,
                PostId = f.pcom.PostId,
                UserName = f.ud.FirstName +" "+ f.ud.LastName,
                Account = f.use.Account,
                CommentContent = f.pcom.CommentContent,
                CommentTime = f.pcom.CommentTime,
                ImagePath = f.ud.ImagePath

            }).ToList();

            var postLikesData = db.PostLikes.Where(s => s.PostLike1 == true).GroupBy(s => s.PostId).Select(f => new LikeModel
            {
                PostId = f.Key,
                TotalLikes = f.Count(p=>p.PostLike1==true),
            }).ToList();

            var postDisLikesData = db.PostLikes.Where(s => s.PostLike1 == false).GroupBy(s => s.PostId).Select(f => new DisLikeModel
            {
                PostId = f.Key,
                TotalDisLikes = f.Count(p => p.PostLike1 == false),
            }).ToList();

            var postCommentLists = new PostViewModel
            {
                postModel = PostResult,
                commentModel = CommentResult,
                likeModel = postLikesData,
                dislikeModel = postDisLikesData


            };
            return postCommentLists;
        }



    }
}