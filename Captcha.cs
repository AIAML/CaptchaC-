public ActionResult CaptchaImage(string prefix, bool noisy = true)
        {
            var rand = new Random((int)DateTime.Now.Ticks);

            //generate new question
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);
            var captcha = string.Format("{0} + {1} = ?", a, b);

            //store answer
            Session["Captcha" + prefix] = a + b;

            //image stream
            FileContentResult img = null;

            using (var mem = new System.IO.MemoryStream())
            using (var bmp = new System.Drawing.Bitmap(130, 30))
            using (var gfx = System.Drawing.Graphics.FromImage((System.Drawing.Image)bmp))
            {
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.FillRectangle(System.Drawing.Brushes.White, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height));

                //add noise
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new System.Drawing.Pen(System.Drawing.Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = System.Drawing.Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);

                        gfx.DrawEllipse(pen, (float)x, (float)y, (float)r, (float)r);
                    }
                }

                //add question
                gfx.DrawString(captcha, new System.Drawing.Font("Tahoma", 15), System.Drawing.Brushes.Gray, 2, 3);

                //render as Jpeg
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");
            }

            return img;
        }
		
		        [HttpPost]
        public ActionResult LoginIndex(UserModel Usermodel)
        {
            Program_RepositoryC Main_RegisteryRepo = new Program_RepositoryC();
            try
            {
                if(Session["Captcha"].ToString() == Usermodel.Captcha)
                {
                    
                    Usermodel.lock_state = -1;
                    Usermodel = userrepo.GetByUandP(Usermodel);
                }
                else
                {
                    ViewBag.Error = '4';
                    return View();
                }
            }
            catch
            {
                ViewBag.Error = '5';
                return View();
            }
            
        }