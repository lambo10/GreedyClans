/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts.UIControllersAndData.Store.Categories.Store;
using UnityEngine;

namespace Assets.Scripts.UIControllersAndData.Images
{

	public class ImageControler
	{
        private static ImageItemList _imageTable;

        /// <summary>
        /// Returns an image by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Sprite GetImage(string id) 
		{
			if (string.IsNullOrEmpty(id))
			{
				return Sprite.Create(null, new Rect(0, 0, 10, 10), new Vector2(0, 0));
			}
			if (_imageTable == null)
			{
				_imageTable = LoadImageTable();
				if (_imageTable == null)
				{
					throw new Exception("Table with images is not loaded");
				}
			}

			var imageItem = _imageTable.ImageList.Find(x => x.TextureId == id);

			return imageItem.Image;
		}

		private static ImageItemList LoadImageTable()
		{
			 return Resources.Load<ImageItemList>(Constants.PATH_FOR_IMAGE_ASSET_LOAD);					
		}
		
	}
}