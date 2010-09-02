<?php

class IgnoreListPager extends AccessListPager
{
	
	function getQueryInfo()
	{
		return array(
			'tables' => 'hmb_user',
			'fields' => array('user_id', 'user_nickname','user_username', 'user_hostname', 'user_accesslevel', 'sort'),
			);
	}
}